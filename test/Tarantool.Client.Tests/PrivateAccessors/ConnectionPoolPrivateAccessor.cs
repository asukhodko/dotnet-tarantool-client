using System.Collections.Generic;
using System.Reflection;

namespace Tarantool.Client.Tests.PrivateAccessors
{
    public static class ConnectionPoolPrivateAccessor
    {
        internal static AcquiredConnection AcquireConnection(this ConnectionPool pool)
        {
            var method = typeof(ConnectionPool).GetMethod("AcquireConnection", BindingFlags.NonPublic | BindingFlags.Instance);
            return (AcquiredConnection)method.Invoke(pool, new object[0]);
        }

        internal static List<ITarantoolConnection> _connections(this ConnectionPool pool)
        {
            var field = typeof(ConnectionPool).GetField("_connections", BindingFlags.NonPublic | BindingFlags.Instance);
            return (List<ITarantoolConnection>)field.GetValue(pool);
        }
    }
}