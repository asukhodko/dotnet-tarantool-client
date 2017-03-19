using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
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
        public async Task ConnectAsync()
        {
            CheckDisposed();
            if (WhenDisconnected != null)
                throw new InvalidOperationException("Connection should be used only once.");
            // ReSharper disable once ExceptionNotDocumented
            var node = _connectionOptions.Nodes[_nodeNumber];
            await _socket.ConnectAsync(node.Host, node.Port);
            _stream = new NetworkStream(_socket, true);
            WhenDisconnected = ReadServerMessagesAsync();
            await HandshakeAsync();
        }

        /// <exception cref="System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException">The stream is currently in use by a previous write operation. </exception>
        /// <exception cref="TarantoolException"></exception>
        public async Task<IList<MessagePackObject>> SelectAsync(uint spaceId, uint indexId)
        {
            CheckDisposed();
            var requestId = _connectionOptions.GetNextRequestId();

            var selectMessage = new SelectRequest(requestId)
            {
                SpaceId = spaceId,
                IndexId = indexId
            };
            // ReSharper disable once ExceptionNotDocumentedOptional
            await _stream.WriteAsync(selectMessage);

            var response = await GetResponseAsync(requestId);
            if (response.IsError)
                throw new TarantoolException(response.ErrorMessage);
            var resultList = response.Body.AsList();
            return resultList;
        }

        /// <exception cref="System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException">The stream is currently in use by a previous write operation. </exception>
        /// <exception cref="TarantoolException"></exception>
        public async Task<IList<MessagePackObject>> EvalAsync(string expression, long[] args)
        {
            CheckDisposed();
            var requestId = _connectionOptions.GetNextRequestId();

            var evalMessage = new EvalRequest(requestId)
            {
                Expression = expression,
                Args = args
            };
            // ReSharper disable once ExceptionNotDocumentedOptional
            await _stream.WriteAsync(evalMessage);

            var response = await GetResponseAsync(requestId);
            if (response.IsError)
                throw new TarantoolException(response.ErrorMessage);
            var resultList = response.Body.AsList();
            return resultList;
        }

        /// <exception cref="ObjectDisposedException"></exception>
        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TarantoolConnection));
        }

        private async Task<Exception> ReadServerMessagesAsync()
        {
            try
            {
                var greetingMessage = await ReadGreetingMessageAsync();
                _greetingServerMessageTcs.SetResult(greetingMessage);
                while (!_isDisposed && _socket.Connected)
                {
                    var message = await _stream.ReadServerMessageAsync();
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

        private async Task HandshakeAsync()
        {
            try
            {
                var greetingMessage = await _greetingServerMessageTcs.Task;

                var user = _connectionOptions.UserName ?? "guest";
                var password = _connectionOptions.Password ?? "";

                var scrambleBytes = CreateScramble(password, greetingMessage.Salt);

                var requestId = _connectionOptions.GetNextRequestId();
                var authMessage = new AuthenticationRequest(requestId)
                {
                    Username = user,
                    Scramble = scrambleBytes
                };
                await _stream.WriteAsync(authMessage);

                var response = await GetResponseAsync(requestId);

                if (response.IsError)
                    throw new TarantoolException(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                _stream.Dispose();
                _stream = null;
                throw new TarantoolException(ex.Message, ex);
            }
        }

        private async Task<ServerMessage> GetResponseAsync(ulong requestId)
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

        private async Task<GreetingServerMessage> ReadGreetingMessageAsync()
        {
            var greetingMessageBytes = await _stream.ReadExactlyBytesAsync(128);
            var greetingMessage = new GreetingServerMessage(greetingMessageBytes);
            if (!greetingMessage.ServerVersion.StartsWith("Tarantool"))
                throw new ProtocolViolationException("This is not a Tarantool server.");
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