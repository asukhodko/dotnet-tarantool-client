using System;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tarantool.Client.Helpers;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
using Tarantool.Client.Models.ServerMessages;

namespace Tarantool.Client
{
    internal class TarantoolConnection : ITarantoolConnection
    {
        private readonly ConnectionOptions _connectionOptions;
        private Socket _socket;
        private NetworkStream _stream;

        public TarantoolConnection(ConnectionOptions connectionOptions)
        {
            _connectionOptions = connectionOptions;
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

        public async Task EnsureConnectedAsync()
        {
            if (_socket == null)
                _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            if (!_socket.Connected)
            {
                // TODO: deal with all nodes
                var node = _connectionOptions.Nodes.First();
                await _socket.ConnectAsync(node.Host, node.Port);
                _stream?.Dispose();
                _stream = new NetworkStream(_socket, true);
                await HandshakeAsync();
            }
        }

        private async Task HandshakeAsync()
        {
            try
            {
                var greetingMessage = await ReadGreetingMessage();

                var user = _connectionOptions.UserName ?? "guest";
                var password = _connectionOptions.Password ?? "";

                var scrambleBytes = CreateScramble(password, greetingMessage.Salt);

                var authMessage = new AuthenticationRequest(user, scrambleBytes, _connectionOptions.GetNextRequestId());
                await _stream.WriteAsync(authMessage);
                var response = await _stream.ReadServerMessage();
                if (response.IsError)
                    throw new Exception(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                _stream.Dispose();
                _stream = null;
                throw new Exception(ex.Message, ex);
            }
        }

        private async Task<GreetingServerMessage> ReadGreetingMessage()
        {
            var greetingMessageBytes = await _stream.ReadExactlyBytesAsync(128);
            var greetingMessage = new GreetingServerMessage(greetingMessageBytes);
            if (!greetingMessage.ServerVersion.StartsWith("Tarantool"))
                throw new Exception("Protocol violation. This is not a Tarantool server.");
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