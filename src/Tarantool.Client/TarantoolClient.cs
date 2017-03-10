using Tarantool.Client.Models;

namespace Tarantool.Client
{
    public class TarantoolClient : ITarantoolClient
    {
        private IConnectionPool _connectionPool;

        public TarantoolClient(string connectionString) : this(new ConnectionOptions(connectionString))
        {
        }

        public TarantoolClient(ConnectionOptions connectionOptions)
        {
            _connectionPool = ConnectionPool.GetPool(connectionOptions);
        }
    }
}