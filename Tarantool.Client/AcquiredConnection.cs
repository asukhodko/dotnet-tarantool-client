using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

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

        public Task<Task<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage, CancellationToken cancellationToken)
        {
            return _connection.RequestAsync(clientMessage, cancellationToken);
        }
    }
}