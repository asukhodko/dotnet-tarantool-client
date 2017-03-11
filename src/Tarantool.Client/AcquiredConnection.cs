using System;

namespace Tarantool.Client
{
    internal class AcquiredConnection : IDisposable, ITarantoolConnection
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

        public void Acquire()
        {
            throw new NotSupportedException("Connection already acquired.");
        }

        public void Release()
        {
            throw new NotSupportedException("Use Dispose() instead.");
        }

        public bool IsAcquired => _connection.IsAcquired;
    }
}