using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tarantool.Client.Models;

namespace Tarantool.Client
{
    internal class ConnectionPool : IConnectionPool
    {
        private static readonly Dictionary<string, IConnectionPool> Pools = new Dictionary<string, IConnectionPool>();

        private readonly ConnectionOptions _connectionOptions;
        private readonly List<ITarantoolConnection> _connections = new List<ITarantoolConnection>();


        private ConnectionPool(ConnectionOptions connectionOptions)
        {
            _connectionOptions = connectionOptions;
        }

        public async Task ConnectAsync()
        {
            using (var connection = AcquireConnection())
            {
                throw new NotImplementedException();
            }
        }

        private AcquiredConnection AcquireConnection()
        {
            return AcquiredConnection.AcquireConnection(_connections, _connectionOptions);
        }

        public static IConnectionPool GetPool(ConnectionOptions connectionOptions)
        {
            var poolKey = connectionOptions.ToString();
            lock (Pools)
            {
                if (Pools.ContainsKey(poolKey))
                    return Pools[poolKey];
                var pool = new ConnectionPool(connectionOptions);
                Pools[poolKey] = pool;
                return pool;
            }
        }

        private class AcquiredConnection : IDisposable
        {
            private readonly ITarantoolConnection _connection;

            private AcquiredConnection(ITarantoolConnection connection)
            {
                _connection = connection;
            }

            public void Dispose()
            {
                _connection.IsAcquired = false;
            }

            public static AcquiredConnection AcquireConnection(List<ITarantoolConnection> connections,
                ConnectionOptions connectionOptions)
            {
                lock (connections)
                {
                    var connection = connections.FirstOrDefault(x => !x.IsAcquired) ??
                                     new TarantoolConnection(connectionOptions);
                    connection.IsAcquired = true;
                    return new AcquiredConnection(connection);
                }
            }
        }
    }
}