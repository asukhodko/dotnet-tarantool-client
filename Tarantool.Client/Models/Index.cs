using System.Collections.Generic;
using MsgPack;
using MsgPack.Serialization;

namespace Tarantool.Client.Models
{
    public class Index
    {
        [MessagePackMember(0)]
        public uint SpaceId { get; set; }

        [MessagePackMember(1)]
        public uint IndexId { get; set; }

        [MessagePackMember(2)]
        public string Name { get; set; }

        [MessagePackMember(3)]
        public IndexType IndexType { get; set; }

        [MessagePackMember(4)]
        public IndexOptions Options { get; set; }

        [MessagePackMember(5)]
        public List<MessagePackObject> Parts { get; set; }
    }
}