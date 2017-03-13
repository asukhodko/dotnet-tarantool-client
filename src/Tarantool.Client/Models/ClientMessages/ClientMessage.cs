using System.IO;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class ClientMessage
    {
        private readonly ClientMessageBodyBase _body;
        private readonly TarantoolCommand _command;
        private readonly ulong _requestId;

        public ClientMessage(TarantoolCommand command, ulong requestId, ClientMessageBodyBase body)
        {
            _command = command;
            _requestId = requestId;
            _body = body;
        }

        public byte[] GetBytes()
        {
            using (var stream = new MemoryStream())
            {
                using (var packer = Packer.Create(stream))
                {
                    PackHeader(packer);
                    _body.Pack(packer);
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
    }
}