using System.Collections.Generic;
using System.Linq;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class UpsertRequest : ClientMessageBase
    {
        public UpsertRequest()
            : base(TarantoolCommand.Upsert)
        {
            Tuple = new List<object>();
            UpdateUperations = new List<UpdateOperation>();
        }

        public uint SpaceId { get; set; }
        public IEnumerable<object> Tuple { get; set; }
        public IEnumerable<UpdateOperation> UpdateUperations { get; set; }

        public override void PackToMessage(Packer packer, PackingOptions options)
        {
            PackHeader(packer);

            packer.PackMapHeader(3);

            packer.Pack((byte)TarantoolKey.Space);
            packer.Pack(SpaceId);

            packer.Pack((byte)TarantoolKey.Tuple);
            packer.PackArray(Tuple);

            packer.Pack((byte)TarantoolKey.UpsertOps);
            packer.PackArrayHeader(UpdateUperations.Count());
            foreach (var operation in UpdateUperations)
            {
                operation.PackToMessage(packer, options);
            }
        }
    }
}