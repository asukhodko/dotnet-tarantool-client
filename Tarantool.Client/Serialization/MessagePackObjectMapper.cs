using MsgPack;

namespace Tarantool.Client.Serialization
{
    public class MessagePackObjectMapper<T>
    {
        public void Map(MessagePackObject source, T target)
        {
            var sourceFields = source.AsList();
        }
    }
}
