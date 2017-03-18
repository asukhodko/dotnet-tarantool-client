using System;
using System.IO;
using System.Threading.Tasks;
using Tarantool.Client.Models.ClientMessages;
using Tarantool.Client.Models.ServerMessages;

namespace Tarantool.Client.Helpers
{
    internal static class StreamExtensions
    {
        public static async Task ReadExactlyBytesAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            var totalReadCount = 0;
            while (totalReadCount < count)
            {
                var readCount = await stream.ReadAsync(buffer, offset + totalReadCount, count - totalReadCount);
                if (readCount == 0)
                    throw new Exception("Unexpected end of stream.");
                totalReadCount += readCount;
            }
        }

        public static async Task<byte[]> ReadExactlyBytesAsync(this Stream stream, int count)
        {
            var buffer = new byte[count];
            await stream.ReadExactlyBytesAsync(buffer, 0, count);
            return buffer;
        }

        public static async Task<ServerMessage> ReadServerMessage(this Stream stream)
        {
            var packedMessageLength = await stream.ReadExactlyBytesAsync(5);
            var unpackedMessageLength = MsgPack.Unpacking.UnpackUInt32(packedMessageLength);
            if(unpackedMessageLength.ReadCount != 5)
                throw new Exception("Protocol violation. Unexpected read bytes count.");
            var messageLength = unpackedMessageLength.Value;
            var messageBytes = await stream.ReadExactlyBytesAsync((int)messageLength);
            return new ServerMessage(messageBytes);
        }

        public static async Task WriteAsync(this Stream stream, ClientMessage message)
        {
            var messageBytes = message.GetBytes();
            var messageLength = messageBytes.Length;
            stream.WriteByte(0xce);
            stream.WriteByte((byte)(messageLength >> 24));
            stream.WriteByte((byte)(messageLength >> 16));
            stream.WriteByte((byte)(messageLength >> 8));
            stream.WriteByte((byte)messageLength);
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
        }

    }
}
