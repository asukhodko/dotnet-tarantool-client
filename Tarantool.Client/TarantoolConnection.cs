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

        public TarantoolConnection(ConnectionOptions connectionOptions, int nodeNumber)
        {
            _connectionOptions = connectionOptions;
            _nodeNumber = nodeNumber;
        }

        public Task<Exception> WhenDisconnected { get; private set; }

        public bool IsConnected => !_isDisposed
                                   && !WhenDisconnected.IsCompleted
                                   && _socket != null
                                   && _socket.Connected;

        /// <exception cref="ObjectDisposedException"></exception>
        public void Dispose()
        {
            CheckDisposed();
            _isDisposed = true;
            _socket?.Dispose();
            _stream?.Dispose();
        }

        public bool IsAcquired { get; private set; }

        public void Acquire()
        {
            IsAcquired = true;
        }

        public void Release()
        {
            IsAcquired = false;
        }

        /// <exception cref="InvalidOperationException">Connection should be used only once.</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">nodeNumber</exception>
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            if (WhenDisconnected != null)
                throw new InvalidOperationException("Connection should be used only once.");
            // ReSharper disable once ExceptionNotDocumented
            var node = _connectionOptions.Nodes[_nodeNumber];
            await _socket.ConnectAsync(node.Host, node.Port);
            _stream = new NetworkStream(_socket, true);
            WhenDisconnected = ReadServerMessagesAsync(cancellationToken);
            await HandshakeAsync(cancellationToken);
        }

        /// <exception cref="System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException">The stream is currently in use by a previous write operation. </exception>
        /// <exception cref="TarantoolResponseException"></exception>
        public async Task<Task<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage,
            CancellationToken cancellationToken)
        {
            CheckDisposed();
            await _stream.WriteAsync(clientMessage, cancellationToken);

            return WhenResponseAsync(clientMessage.RequestId)
                .ContinueWith(t =>
                {
                    var response = t.Result;
                    if (response.IsError)
                        throw new TarantoolResponseException(response.ErrorMessage, response.Code);
                    return response.Body;
                }, cancellationToken);
        }

        /// <exception cref="ObjectDisposedException"></exception>
        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TarantoolConnection));
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

        private TaskCompletionSource<ServerMessage> GetServerMessageTcs(ulong requestId)
        {
            lock (_serverMessagesTcss)
            {
                if (_serverMessagesTcss.ContainsKey(requestId))
                    return _serverMessagesTcss[requestId];
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
                var password = _connectionOptions.Password ?? "";

                var scrambleBytes = CreateScramble(password, greetingMessage.Salt);

                await await RequestAsync(new AuthenticationRequest
                {
                    Username = user,
                    Scramble = scrambleBytes
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _stream.Dispose();
                _stream = null;
                throw new TarantoolException(ex.Message, ex);
            }
        }

        private async Task<ServerMessage> WhenResponseAsync(ulong requestId)
        {
            var messageTcs = GetServerMessageTcs(requestId);
            await Task.WhenAny(messageTcs.Task, WhenDisconnected);
            if (!messageTcs.Task.IsCompleted)
                throw new TarantoolException("Connection closed.");
            lock (_serverMessagesTcss)
            {
                _serverMessagesTcss.Remove(requestId);
            }
            return messageTcs.Task.Result;
        }

        private async Task<GreetingServerMessage> ReadGreetingMessageAsync(CancellationToken cancellationToken)
        {
            var greetingMessageBytes = await _stream.ReadExactlyBytesAsync(128, cancellationToken);
            var greetingMessage = new GreetingServerMessage(greetingMessageBytes);
            if (!greetingMessage.ServerVersion.StartsWith("Tarantool"))
                throw new TarantoolProtocolViolationException("This is not a Tarantool server.");
            return greetingMessage;
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
            for (var i = 0; i < step3.Length; i++)
                step3[i] ^= step1[i];
            return step3;
        }
    }
}