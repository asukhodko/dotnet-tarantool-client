using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MsgPack;

using Tarantool.Client.Helpers;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
using Tarantool.Client.Models.ServerMessages;

namespace Tarantool.Client
{
    /// <summary>
    ///     The tarantool connection.
    /// </summary>
    internal class TarantoolConnection : ITarantoolConnection
    {
        private readonly ConnectionOptions _connectionOptions;

        private readonly TaskCompletionSource<GreetingServerMessage> _greetingServerMessageTcs =
            new TaskCompletionSource<GreetingServerMessage>();

        private readonly int _nodeNumber;

        private readonly Dictionary<ulong, TaskCompletionSource<ServerMessage>> _serverMessagesTcss =
            new Dictionary<ulong, TaskCompletionSource<ServerMessage>>();

        private readonly Socket _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        private bool _isDisposed;

        private NetworkStream _stream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TarantoolConnection" /> class.
        /// </summary>
        /// <param name="connectionOptions">The connection options.</param>
        /// <param name="nodeNumber">The node number.</param>
        public TarantoolConnection(ConnectionOptions connectionOptions, int nodeNumber)
        {
            _connectionOptions = connectionOptions;
            _nodeNumber = nodeNumber;
        }

        /// <summary>Gets a value indicating whether connection is now in use for sending request.</summary>
        public bool IsAcquired { get; private set; }

        /// <summary>Gets a value indicating whether connection is still active.</summary>
        public bool IsConnected => !_isDisposed && !WhenDisconnected.IsCompleted && _socket != null
                                   && _socket.Connected;

        private Task<Exception> WhenDisconnected { get; set; }

        /// <summary>Mark connection as in use for sending request.</summary>
        public void Acquire()
        {
            IsAcquired = true;
        }

        /// <summary>Connect to Tarantool and authenticate.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        /// <exception cref="InvalidOperationException">Connection should be used only once.</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">nodeNumber</exception>
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            if (WhenDisconnected != null) throw new InvalidOperationException("Connection should be used only once.");

            // ReSharper disable once ExceptionNotDocumented
            var node = _connectionOptions.Nodes[_nodeNumber];
            await _socket.ConnectAsync(node.Host, node.Port).ConfigureAwait(false);
            _stream = new NetworkStream(_socket, true);
            WhenDisconnected = ReadServerMessagesAsync(cancellationToken);
            await HandshakeAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Closes the connection and releases resources.</summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public void Dispose()
        {
            CheckDisposed();
            _isDisposed = true;
            _socket?.Dispose();
            _stream?.Dispose();
        }

        /// <summary>Schedules the continuation action when disconnection happens.</summary>
        /// <param name="continuation">The action to invoke on disconnection.</param>
        public void OnDisconnected(Action continuation)
        {
            WhenDisconnected.GetAwaiter().OnCompleted(continuation);
        }

        /// <summary>Mark connection as not in use for sending request.</summary>
        public void Release()
        {
            IsAcquired = false;
        }

        /// <summary>Send request to Tarantool server.</summary>
        /// <param name="clientMessage">The client message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> for awaiting the result.</returns>
        /// <exception cref="System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException">The stream is currently in use by a previous write operation. </exception>
        /// <exception cref="TarantoolResponseException"></exception>
        public async Task<Task<MessagePackObject>> RequestAsync(
            ClientMessageBase clientMessage,
            CancellationToken cancellationToken)
        {
            CheckDisposed();
            await _stream.WriteAsync(clientMessage, cancellationToken).ConfigureAwait(false);

            return WhenResponseAsync(clientMessage.RequestId)
                .ContinueWith(
                    t =>
                        {
                            var response = t.Result;
                            if (response.IsError)
                                throw new TarantoolResponseException(response.ErrorMessage, response.Code);
                            return response.Body;
                        },
                    cancellationToken);
        }

        private static byte[] CreateScramble(string password, byte[] salt)
        {
            var sha1 = SHA1.Create();
            var step1 = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            var step2 = sha1.ComputeHash(step1);
            var step3Input = new byte[20 + step2.Length];
            Buffer.BlockCopy(salt, 0, step3Input, 0, 20);
            Buffer.BlockCopy(step2, 0, step3Input, 20, step2.Length);
            var step3 = sha1.ComputeHash(step3Input);
            for (var i = 0; i < step3.Length; i++) step3[i] ^= step1[i];
            return step3;
        }

        /// <summary>Raises exception if object is already disposed.</summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void CheckDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TarantoolConnection));
        }

        private TaskCompletionSource<ServerMessage> GetServerMessageTcs(ulong requestId)
        {
            lock (_serverMessagesTcss)
            {
                if (_serverMessagesTcss.ContainsKey(requestId)) return _serverMessagesTcss[requestId];
                var tcs = new TaskCompletionSource<ServerMessage>();
                _serverMessagesTcss[requestId] = tcs;
                return tcs;
            }
        }

        private async Task HandshakeAsync(CancellationToken cancellationToken)
        {
            try
            {
                var greetingMessage = await _greetingServerMessageTcs.Task;

                var user = _connectionOptions.UserName ?? "guest";
                var password = _connectionOptions.Password ?? string.Empty;

                var scrambleBytes = CreateScramble(password, greetingMessage.Salt);

                await await RequestAsync(
                          new AuthenticationRequest { Username = user, Scramble = scrambleBytes },
                          cancellationToken);
            }
            catch (Exception ex)
            {
                _stream.Dispose();
                _stream = null;
                throw new TarantoolException(ex.Message, ex);
            }
        }

        private async Task<GreetingServerMessage> ReadGreetingMessageAsync(CancellationToken cancellationToken)
        {
            var greetingMessageBytes = await _stream.ReadExactlyBytesAsync(128, cancellationToken);
            var greetingMessage = new GreetingServerMessage(greetingMessageBytes);
            if (!greetingMessage.ServerVersion.StartsWith("Tarantool"))
                throw new TarantoolProtocolViolationException("This is not a Tarantool server.");
            return greetingMessage;
        }

        private async Task<Exception> ReadServerMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var greetingMessage = await ReadGreetingMessageAsync(cancellationToken);
                _greetingServerMessageTcs.SetResult(greetingMessage);
                while (!_isDisposed && _socket.Connected)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var message = await _stream.ReadServerMessageAsync(cancellationToken);
                    if (message.RequestId == 0)
                        throw new TarantoolException(
                            $"Unrecognized server message. Code={message.Code}, ErrorMessage={message.ErrorMessage}.");
                    var messageTcs = GetServerMessageTcs(message.RequestId);
                    messageTcs.SetResult(message);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                    // ignored
                }

                return ex;
            }

            return new TarantoolException("Connection closed.");
        }

        private async Task<ServerMessage> WhenResponseAsync(ulong requestId)
        {
            var messageTcs = GetServerMessageTcs(requestId);
            await Task.WhenAny(messageTcs.Task, WhenDisconnected);
            if (!messageTcs.Task.IsCompleted) throw new TarantoolException("Connection closed.");
            lock (_serverMessagesTcss)
            {
                _serverMessagesTcss.Remove(requestId);
            }

            return messageTcs.Task.Result;
        }
    }
}