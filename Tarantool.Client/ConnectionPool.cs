using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MsgPack;
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
            using (await AcquireConnectionAsync())
            {
                
            }
        }

        public async Task<IList<MessagePackObject>> EvalAsync(string expression, long[] args)
        {
            using (var connection = await AcquireConnectionAsync())
            {
                return await connection.EvalAsync(expression, args);
            }
        }

        private async Task<IAcquiredConnection> AcquireConnectionAsync()
        {
            IAcquiredConnection acquiredConnection;
            ITarantoolConnection newTarantoolConnection = null;
            lock (_connections)
            {
                var connection = _connections.FirstOrDefault(x => !x.IsAcquired);
                if (connection == null)
                {
                    newTarantoolConnection = new TarantoolConnection(_connectionOptions, 0);
                    _connections.Add(newTarantoolConnection);
                    connection = newTarantoolConnection;
                }
                acquiredConnection = new AcquiredConnection(connection);
            }
            if (newTarantoolConnection != null)
            {
                await PrepareConnectionAsync(newTarantoolConnection);
            }
            return acquiredConnection;
        }

        private async Task PrepareConnectionAsync(ITarantoolConnection connection)
        {
            await connection.ConnectAsync();
            connection.WhenDisconnected.ConfigureAwait(false).GetAwaiter().OnCompleted(() =>
            {
                lock (_connections)
                {
                    _connections.Remove(connection);
                    try
                    {
                        connection.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });
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

        public void Dispose()
        {
            foreach (var connection in _connections)
            {
                try
                {
                    connection.Dispose();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}