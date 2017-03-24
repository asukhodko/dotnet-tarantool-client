using System.Collections.Generic;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class ReplaceRequest : ClientMessageBase
    {
        public ReplaceRequest()
            : base(TarantoolCommand.Replace)
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