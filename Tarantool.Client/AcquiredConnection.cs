using System.Collections.Generic;
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

        public Task<IList<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage)
        {
            return _connection.RequestAsync(clientMessage);
        }
    }
}