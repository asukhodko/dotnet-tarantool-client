using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Tarantool.Client.Tests.PrivateAccessors
{
    public static class ConnectionPoolPrivateAccessor
    {
        internal static Task<IAcquiredConnection> AcquireConnectionAsync(this ConnectionPool pool, CancellationToken cancellationToken = default(CancellationToken))
        {
            var method = typeof(ConnectionPool).GetMethod("AcquireConnectionAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Task<IAcquiredConnection>)method.Invoke(pool, new object[] { cancellationToken });
        }

        internal static List<ITarantoolConnection> _connections(this ConnectionPool pool)
        {
            var field = typeof(ConnectionPool).GetField("_connections", BindingFlags.NonPublic | BindingFlags.Instance);
            return (List<ITarantoolConnection>)field.GetValue(pool);
        }
    }
}