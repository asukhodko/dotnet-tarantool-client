using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;

namespace Tarantool.Client
{
    internal class AcquiredConnection : IAcquiredConnection
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

        public Task<IList<MessagePackObject>> EvalAsync(string expression, long[] args)
        {
            return _connection.EvalAsync(expression, args);
        }
    }
}