namespace Tarantool.Client
{
    public class TarantoolSpace<T> : ITarantoolSpace<T>
    {
        private readonly uint _spaceId;
        private readonly string _spaceName;

        public TarantoolSpace(ITarantoolClient tarantoolClient, uint spaceId)
        {
            TarantoolClient = tarantoolClient;
            _spaceId = spaceId;
        }

        public TarantoolSpace(ITarantoolClient tarantoolClient, string spaceName)
        {
            TarantoolClient = tarantoolClient;
            _spaceName = spaceName;
        }

        public ITarantoolClient TarantoolClient { get; }
    }
}