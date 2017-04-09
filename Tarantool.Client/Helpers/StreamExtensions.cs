using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;
using Tarantool.Client.Models.ServerMessages;

namespace Tarantool.Client.Helpers
{
    internal static class StreamExtensions
    {
        private static readonly object RequestMutex = new object();
        private static ulong _nextRequestId = 1000;

        /// <exception cref="EndOfStreamException">Unexpected end of stream.</exception>
        public static async Task ReadExactlyBytesAsync(this Stream stream, byte[] buffer, int offset, int count,
            CancellationToken cancellationToken)
        {
            var totalReadCount = 0;
            while (totalReadCount < count)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var readCount = await stream.ReadAsync(buffer, offset + totalReadCount, count - totalReadCount,
                    cancellationToken);
                if (readCount == 0)
                    throw new EndOfStreamException("Unexpected end of stream.");
                totalReadCount += readCount;
            }
        }

        /// <exception cref="EndOfStreamException">Unexpected end of stream.</exception>
        public static async Task<byte[]> ReadExactlyBytesAsync(this Stream stream, int count,
            CancellationToken cancellationToken)
        {
            var buffer = new byte[count];
            await stream.ReadExactlyBytesAsync(buffer, 0, count, cancellationToken);
            return buffer;
        }

        /// <exception cref="EndOfStreamException">Unexpected end of stream.</exception>
        /// <exception cref="TarantoolProtocolViolationException">Unexpected read bytes count.</exception>
        /// <exception cref="UnpackException">
        ///     source is not valid MessagePack stream.
        /// </exception>
        /// <exception cref="MessageTypeException">
        ///     The unpacked result is not compatible to <see cref="T:System.UInt32" />.
        /// </exception>
        public static async Task<ServerMessage> ReadServerMessageAsync(this Stream stream,
            CancellationToken cancellationToken)
        {
            var packedMessageLength = await stream.ReadExactlyBytesAsync(5, cancellationToken);
            var unpackedMessageLength = Unpacking.UnpackUInt32(packedMessageLength);
            if (unpackedMessageLength.ReadCount != 5)
                throw new TarantoolProtocolViolationException("Unexpected read bytes count.");
            var messageLength = unpackedMessageLength.Value;
            var messageBytes = await stream.ReadExactlyBytesAsync((int)messageLength, cancellationToken);
            var serverMessage = new ServerMessage(messageBytes);
            return serverMessage;
        }

        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <exception cref="System.NotSupportedException">The stream does not support writing, or the stream is already closed. </exception>
        /// <exception cref="System.InvalidOperationException">The stream is currently in use by a previous write operation. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public static async Task WriteAsync(this Stream stream, ClientMessageBase message,
            CancellationToken cancellationToken)
        {
            message.RequestId = GetNextRequestId();

            byte[] messageBytes;

            using (var ms = new MemoryStream())
            {
                using (var packer = Packer.Create(ms))
                {
                    message.PackToMessage(packer, null);
                    messageBytes = ms.ToArray();
                }
            }

            // ReSharper disable once ExceptionNotDocumented
            var messageLength = messageBytes.Length;
            stream.WriteByte(0xce);
            stream.WriteByte((byte)(messageLength >> 24));
            stream.WriteByte((byte)(messageLength >> 16));
            stream.WriteByte((byte)(messageLength >> 8));
            stream.WriteByte((byte)messageLength);
            // ReSharper disable once ExceptionNotDocumented
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
        }

        private static ulong GetNextRequestId()
        {
            lock (RequestMutex)
            {
                return _nextRequestId++;
            }
        }
    }
}