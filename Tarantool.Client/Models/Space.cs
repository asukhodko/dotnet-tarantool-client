using System.Collections.Generic;
using MsgPack;
using Tarantool.Client.Serialization.Attributes;

namespace Tarantool.Client.Models
{
    public class Space
    {
        public Space()
        {
            Format = new List<MessagePackObject>();
        }

        [TarantoolField(0)]
        public uint Id { get; set; }

        [TarantoolField(1)]
        public uint OwnerId { get; set; }

        [TarantoolField(2)]
        public string Name { get; set; }

        [TarantoolField(3)]
        public StorageEngine Engine { get; set; }

        [TarantoolField(4)]
        public uint FieldCount { get; set; }

        [TarantoolField(5)]
        public string Flags { get; set; }

        [TarantoolField(6)]
        public List<MessagePackObject> Format { get; set; }
    }
}