namespace Tarantool.Client
{
    internal class AcquiredConnection : IAcquiredConnection
    {
        private readonly ITarantoolConnection _connection;

        internal AcquiredConnection(ITarantoolConnection connection)
        {
            _connection = connection;
            _connection.Acquire();
        }

        public void Dispose()
        {
            _connection.Release();
        }
    }
}