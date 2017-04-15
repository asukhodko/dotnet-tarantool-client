using System.Collections.Generic;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class InsertRequest<T> : ClientMessageBase
    {
        public InsertRequest()
            : base(TarantoolCommand.Insert)
        {
        }

        public uint SpaceId { get; set; }
        public T Tuple { get; set; }

        public override void PackToMessage(Packer packer, PackingOptions options)
        {
            PackHeader(packer);

            packer.PackMapHeader(2);

            packer.Pack((byte)TarantoolKey.Space);
            packer.Pack(SpaceId);

            packer.Pack((byte)TarantoolKey.Tuple);
            packer.Pack(Tuple);
        }
    }
}