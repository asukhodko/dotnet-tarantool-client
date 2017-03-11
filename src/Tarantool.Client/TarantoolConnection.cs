using Tarantool.Client.Models;

namespace Tarantool.Client
{
    internal class TarantoolConnection : ITarantoolConnection
    {
        private ConnectionOptions _connectionOptions;

        public TarantoolConnection(ConnectionOptions connectionOptions)
        {
            _connectionOptions = connectionOptions;
        }

        public bool IsAcquired { get; set; }
    }
}