using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MsgPack;

using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    /// <summary>The connection pool.</summary>
    internal class ConnectionPool : IConnectionPool
    {
        /// <summary>The static pools for connection strings.</summary>
        private static readonly Dictionary<string, IConnectionPool> Pools = new Dictionary<string, IConnectionPool>();

        /// <summary>The options for this pool.</summary>
        private readonly ConnectionOptions _connectionOptions;

        /// <summary>The connections of this pool.</summary>
        private readonly List<ITarantoolConnection> _connections = new List<ITarantoolConnection>();

        /// <summary>Initializes a new instance of the <see cref="ConnectionPool" /> class.</summary>
        /// <param name="connectionOptions">The connection options.</param>
        private ConnectionPool(ConnectionOptions connectionOptions)
        {
            _connectionOptions = connectionOptions;
        }

        /// <summary>Find an appropriate instance of the <see cref="IConnectionPool" /> or creates new one.</summary>
        /// <param name="connectionOptions">The connection options.</param>
        /// <returns>The <see cref="IConnectionPool" />.</returns>
        public static IConnectionPool GetPool(ConnectionOptions connectionOptions)
        {
            var poolKey = connectionOptions.ToString();
            lock (Pools)
            {
                if (Pools.ContainsKey(poolKey)) return Pools[poolKey];
                var pool = new ConnectionPool(connectionOptions);
                Pools[poolKey] = pool;
                return pool;
            }
        }

        /// <summary>
        ///     Establish a connection with Tarantool and authenticate. Calling this method is redundant because connection
        ///     establishes automatically during 1st request.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            using (await AcquireConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
            }
        }

        /// <summary>
        ///     Close (and abort) all connections with Tarantool.
        ///     Usually you do not need to dispose the connection pool as it shares between task and threads.
        /// </summary>
        public void Dispose()
        {
            foreach (var connection in _connections)
                try
                {
                    connection.Dispose();
                }
                catch
                {
                    // ignored
                }
        }

        /// <summary>Send request to Tarantool server and get response.</summary>
        /// <param name="clientMessage">The client message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with response.</returns>
        public async Task<MessagePackObject> RequestAsync(
            ClientMessageBase clientMessage,
            CancellationToken cancellationToken)
        {
            Task<MessagePackObject> resultTask;
            using (var connection = await AcquireConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                resultTask = await connection.RequestAsync(clientMessage, cancellationToken).ConfigureAwait(false);
            }

            return await resultTask.ConfigureAwait(false);
        }

        /// <summary>Acquire and lock connection from a pool.</summary>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        private async Task<IAcquiredConnection> AcquireConnectionAsync(CancellationToken cancellationToken)
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
                await PrepareNewConnectionAsync(newTarantoolConnection, cancellationToken).ConfigureAwait(false);

            return acquiredConnection;
        }

        /// <summary>Connect to Tarantool and watch for disconnection, remove from pool on disconnect.</summary>
        /// <param name="newConnection">The new connection.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task PrepareNewConnectionAsync(
            ITarantoolConnection newConnection,
            CancellationToken cancellationToken)
        {
            await newConnection.ConnectAsync(cancellationToken).ConfigureAwait(false);
            newConnection.WhenDisconnected.ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(
                    () =>
                        {
                            lock (_connections)
                            {
                                _connections.Remove(newConnection);
                                try
                                {
                                    newConnection.Dispose();
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        });
        }
    }
}