using System.Collections.Generic;
using MsgPack;
using MsgPack.Serialization;

namespace Tarantool.Client.Models
{
    public class Space
    {
        public Space()
        {
            Format = new List<MessagePackObject>();
        }

        [MessagePackMember(1)]
        public uint OwnerId { get; set; }

        [MessagePackMember(0)]
        public uint SpaceId { get; set; }

        [MessagePackMember(2)]
        public string Name { get; set; }

        //[MessagePackMember(3)]
        public StorageEngine Engine { get; set; }

        [MessagePackMember(4)]
        public uint FieldCount { get; set; }

        //[MessagePackMember(5)]
        public string Flags { get; set; }

        //[MessagePackMember(6)]
        public List<MessagePackObject> Format { get; set; }
    }
}