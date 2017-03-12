using System.IO;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class ClientMessageHeader
    {
        public ClientMessageHeader(TarantoolCommand command, ulong requestId)
        {
            Command = command;
            RequestId = requestId;
        }

        public TarantoolCommand Command { get; }
        public ulong RequestId { get; }

        public void Pack(Packer packer)
        {
            packer.PackMapHeader(2);

            packer.Pack((byte)TarantoolKey.Code);
            packer.Pack((byte)Command);

            packer.Pack((byte)TarantoolKey.Sync);
            packer.Pack(RequestId);
        }
    }
}