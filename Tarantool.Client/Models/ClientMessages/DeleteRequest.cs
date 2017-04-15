using System.Collections.Generic;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class DeleteRequest : ClientMessageBase
    {
        public DeleteRequest()
            : base(TarantoolCommand.Delete)
        {
            Key = new List<object>();
        }

        public uint SpaceId { get; set; }
        public uint IndexId { get; set; }
        public IEnumerable<object> Key { get; set; }

        public override void PackToMessage(Packer packer, PackingOptions options)
        {
            PackHeader(packer);

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