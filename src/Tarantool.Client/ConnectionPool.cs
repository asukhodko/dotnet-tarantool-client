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
            lock (_connections)
            {
                var connection = _connections.FirstOrDefault(x => !x.IsAcquired);
                if (connection == null)
                {
                    connection = new TarantoolConnection(_connectionOptions);
                    _connections.Add(connection);
                }
                return new AcquiredConnection(connection);
            }
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
    }
}