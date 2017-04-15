using System.Collections.Generic;
using MsgPack;
using MsgPack.Serialization;

namespace Tarantool.Client.Models
{
    public class Space
    {
        public Space()
        {
           Format = new List<Field>();
        }

        [MessagePackMember(1)]
        public uint OwnerId { get; set; }

        [MessagePackMember(0)]
        public uint SpaceId { get; set; }

        [MessagePackMember(2)]
        public string Name { get; set; }

        [MessagePackMember(3)]
        public StorageEngine Engine { get; set; }

        [MessagePackMember(4)]
        public uint FieldCount { get; set; }

        [MessagePackMember(5)]
        public MessagePackObject Flags { get; set; }

        [MessagePackMember(6)]
        public List<Field> Format { get; set; }
    }
}