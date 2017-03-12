using System.IO;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class ClientMessage
    {
        private readonly ClientMessageBodyBase _body;
        private readonly ClientMessageHeader _header;

        public ClientMessage(ClientMessageHeader header, ClientMessageBodyBase body)
        {
            _header = header;
            _body = body;
        }

        public byte[] GetBytes()
        {
            using (var stream = new MemoryStream())
            {
                using (var packer = Packer.Create(stream))
                {
                    _header.Pack(packer);
                    _body.Pack(packer);
                    return stream.ToArray();
                }
            }
        }
    }
}