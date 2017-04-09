using System.Collections.Generic;
using System.Linq;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class UpsertRequest<T> : ClientMessageBase
    {
        public UpsertRequest()
            : base(TarantoolCommand.Upsert)
        {
            UpdateOperations = new List<UpdateOperation>();
        }

        public uint SpaceId { get; set; }
        public T Tuple { get; set; }
        public IEnumerable<UpdateOperation> UpdateOperations { get; set; }

        public override void PackToMessage(Packer packer, PackingOptions options)
        {
            PackHeader(packer);

            packer.PackMapHeader(3);

            packer.Pack((byte)TarantoolKey.Space);
            packer.Pack(SpaceId);

            packer.Pack((byte)TarantoolKey.Tuple);
            packer.Pack(Tuple);

            packer.Pack((byte)TarantoolKey.UpsertOps);
            packer.PackArrayHeader(UpdateOperations.Count());
            foreach (var operation in UpdateOperations)
            {
                operation.PackToMessage(packer, options);
            }
        }
    }
}