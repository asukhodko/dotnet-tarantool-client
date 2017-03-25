using System.IO;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public abstract class ClientMessageBase : IPackable
    {
        protected ClientMessageBase(TarantoolCommand command)
        {
            Command = command;
        }

        public TarantoolCommand Command { get; }

        public ulong RequestId { get; set; }

        protected void PackHeader(Packer packer)
        {
            packer.PackMapHeader(2);

            packer.Pack((byte)TarantoolKey.Code);
            packer.Pack((byte)Command);

            packer.Pack((byte)TarantoolKey.Sync);
            packer.Pack(RequestId);
        }

        public abstract void PackToMessage(Packer packer, PackingOptions options);
    }
}