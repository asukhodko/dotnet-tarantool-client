using System.IO;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public abstract class ClientMessageBase
    {
        private static readonly object RequestMutex = new object();
        private static ulong _nextRequestId = 1000;

        protected ClientMessageBase(TarantoolCommand command)
        {
            Command = command;
        }

        public TarantoolCommand Command { get; }

        public ulong RequestId { get; private set; }

        private static ulong GetNextRequestId()
        {
            lock (RequestMutex)
            {
                return _nextRequestId++;
            }
        }

        public byte[] GetBytes()
        {
            RequestId = GetNextRequestId();
            using (var stream = new MemoryStream())
            {
                using (var packer = Packer.Create(stream))
                {
                    PackHeader(packer);
                    PackBody(packer);
                    return stream.ToArray();
                }
            }
        }

        private void PackHeader(Packer packer)
        {
            packer.PackMapHeader(2);

            packer.Pack((byte)TarantoolKey.Code);
            packer.Pack((byte)Command);

            packer.Pack((byte)TarantoolKey.Sync);
            packer.Pack(RequestId);
        }

        protected abstract void PackBody(Packer packer);
    }
}