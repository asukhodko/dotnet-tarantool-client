using System.Collections.Generic;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class DeletetRequest : ClientMessageBase
    {
        public DeletetRequest()
            : base(TarantoolCommand.Delete)
        {
            Key = new List<object>();
        }

        public uint SpaceId { get; set; }
        public uint IndexId { get; set; }
        public IEnumerable<object> Key { get; set; }

        protected override void PackBody(Packer packer)
        {
            packer.PackMapHeader(3);

            packer.Pack((byte)TarantoolKey.Space);
            packer.Pack(SpaceId);

            packer.Pack((byte)TarantoolKey.Index);
            packer.Pack(IndexId);

            packer.Pack((byte)TarantoolKey.Key);
            packer.PackArray(Key);
        }
    }
}