using System.Collections.Generic;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class InsertRequest : ClientMessageBase
    {
        public InsertRequest()
            : base(TarantoolCommand.Insert)
        {
            Tuple = new List<object>();
        }

        public uint SpaceId { get; set; }
        public IEnumerable<object> Tuple { get; set; }

        protected override void PackBody(Packer packer)
        {
            packer.PackMapHeader(2);

            packer.Pack((byte)TarantoolKey.Space);
            packer.Pack(SpaceId);

            packer.Pack((byte)TarantoolKey.Tuple);
            packer.PackArray(Tuple);
        }
    }
}