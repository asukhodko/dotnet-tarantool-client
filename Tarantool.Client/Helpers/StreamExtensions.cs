using System.IO;
using System.Net;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;
using Tarantool.Client.Models.ServerMessages;

namespace Tarantool.Client.Helpers
{
    internal static class StreamExtensions
    {
        /// <exception cref="EndOfStreamException">Unexpected end of stream.</exception>
        public static async Task ReadExactlyBytesAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            var totalReadCount = 0;
            while (totalReadCount < count)
            {
                var readCount = await stream.ReadAsync(buffer, offset + totalReadCount, count - totalReadCount);
                if (readCount == 0)
                    throw new EndOfStreamException("Unexpected end of stream.");
                totalReadCount += readCount;
            }
        }

        /// <exception cref="EndOfStreamException">Unexpected end of stream.</exception>
        public static async Task<byte[]> ReadExactlyBytesAsync(this Stream stream, int count)
        {
            var buffer = new byte[count];
            await stream.ReadExactlyBytesAsync(buffer, 0, count);
            return buffer;
        }

        /// <exception cref="EndOfStreamException">Unexpected end of stream.</exception>
        /// <exception cref="ProtocolViolationException">Unexpected read bytes count.</exception>
        /// <exception cref="UnpackException">
        ///     source is not valid MessagePack stream.
        /// </exception>
        /// <exception cref="MessageTypeException">
        ///     The unpacked result is not compatible to <see cref="T:System.UInt32" />.
        /// </exception>
        public static async Task<ServerMessage> ReadServerMessageAsync(this Stream stream)
        {
            var packedMessageLength = await stream.ReadExactlyBytesAsync(5);
            var unpackedMessageLength = Unpacking.UnpackUInt32(packedMessageLength);
            if (unpackedMessageLength.ReadCount != 5)
                throw new ProtocolViolationException("Unexpected read bytes count.");
            var messageLength = unpackedMessageLength.Value;
            var messageBytes = await stream.ReadExactlyBytesAsync((int)messageLength);
            return new ServerMessage(messageBytes);
        }

        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <exception cref="System.NotSupportedException">The stream does not support writing, or the stream is already closed. </exception>
        /// <exception cref="System.InvalidOperationException">The stream is currently in use by a previous write operation. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public static async Task WriteAsync(this Stream stream, ClientMessage message)
        {
            var messageBytes = message.GetBytes();
            // ReSharper disable once ExceptionNotDocumented
            var messageLength = messageBytes.Length;
            stream.WriteByte(0xce);
            stream.WriteByte((byte)(messageLength >> 24));
            stream.WriteByte((byte)(messageLength >> 16));
            stream.WriteByte((byte)(messageLength >> 8));
            stream.WriteByte((byte)messageLength);
            // ReSharper disable once ExceptionNotDocumented
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
        }
    }
}