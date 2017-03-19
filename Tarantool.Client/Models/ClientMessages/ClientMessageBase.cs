using System.IO;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public abstract class ClientMessageBase
    {
        private readonly TarantoolCommand _command;
        private readonly ulong _requestId;

        protected ClientMessageBase(TarantoolCommand command, ulong requestId)
        {
            _command = command;
            _requestId = requestId;
        }

        public byte[] GetBytes()
        {
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
            packer.Pack((byte)_command);

            packer.Pack((byte)TarantoolKey.Sync);
            packer.Pack(_requestId);
        }

        protected abstract void PackBody(Packer packer);
    }
}