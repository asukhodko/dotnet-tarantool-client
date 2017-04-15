using System.Collections.Generic;
using System.Linq;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class UpdateRequest : ClientMessageBase
    {
        public UpdateRequest()
            : base(TarantoolCommand.Update)
        {
            Key = new List<object>();
            UpdateOperations = new List<UpdateOperation>();
        }

        public uint SpaceId { get; set; }
        public uint IndexId { get; set; }
        public IEnumerable<object> Key { get; set; }
        public IEnumerable<UpdateOperation> UpdateOperations { get; set; }

        public override void PackToMessage(Packer packer, PackingOptions options)
        {
            PackHeader(packer);

            packer.PackMapHeader(4);

            packer.Pack((byte)TarantoolKey.Space);
            packer.Pack(SpaceId);

            packer.Pack((byte)TarantoolKey.Index);
            packer.Pack(IndexId);

            packer.Pack((byte)TarantoolKey.Key);
            packer.PackArray(Key);

            packer.Pack((byte)TarantoolKey.Tuple);
            packer.PackArrayHeader(UpdateOperations.Count());
            foreach (var operation in UpdateOperations)
            {
                operation.PackToMessage(packer, options);
            }
        }
    }
}