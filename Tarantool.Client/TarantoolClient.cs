using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

[assembly: InternalsVisibleTo("Tarantool.Client.Tests")]

namespace Tarantool.Client
{
    public class TarantoolClient : ITarantoolClient
    {
        private readonly IConnectionPool _connectionPool;

        public TarantoolClient(string connectionString) : this(new ConnectionOptions(connectionString))
        {
        }

        public TarantoolClient(ConnectionOptions connectionOptions)
        {
            _connectionPool = ConnectionPool.GetPool(connectionOptions);
        }

        public async Task ConnectAsync()
        {
            await _connectionPool.ConnectAsync();
        }

        public async Task<IList<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage)
        {
            return await _connectionPool.RequestAsync(clientMessage);
        }
        
    }
}