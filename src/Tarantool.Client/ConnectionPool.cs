using System.Collections.Generic;
using Tarantool.Client.Models;

namespace Tarantool.Client
{
    public class ConnectionPool : IConnectionPool
    {
        private static readonly Dictionary<string, IConnectionPool> Pools = new Dictionary<string, IConnectionPool>();
        private ConnectionOptions _connectionOptions;

        private ConnectionPool(ConnectionOptions connectionOptions)
        {
            _connectionOptions = connectionOptions;
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