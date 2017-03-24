using System;
using System.Collections.Generic;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class SelectRequest : ClientMessageBase
    {
        public SelectRequest()
            : base(TarantoolCommand.Select)
        {
            Iterator = Iterator.Eq;
            Limit = int.MaxValue;
            Key = new List<object>();
        }

        public uint SpaceId { get; set; }
        public uint IndexId { get; set; }
        public uint Limit { get; set; }
        public uint Offset { get; set; }
        public Iterator Iterator { get; set; }
        public IEnumerable<object> Key { get; set; }

        public override void PackToMessage(Packer packer, PackingOptions options)
        {
            PackHeader(packer);

            packer.PackMapHeader(6);

            packer.Pack((byte)TarantoolKey.Space);
            packer.Pack(SpaceId);

            packer.Pack((byte)TarantoolKey.Index);
            packer.Pack(IndexId);

            packer.Pack((byte)TarantoolKey.Limit);
            packer.Pack(Limit);

            packer.Pack((byte)TarantoolKey.Offset);
            packer.Pack(Offset);

            packer.Pack((byte)TarantoolKey.Iterator);
            packer.Pack((uint)Iterator);

            packer.Pack((byte)TarantoolKey.Key);
            packer.PackArray(Key);
        }
    }
}