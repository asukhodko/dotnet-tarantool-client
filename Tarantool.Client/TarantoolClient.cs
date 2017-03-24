using System.Collections.Generic;
using System.Linq;
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
        private const int VSpaceSpaceId = 281;
        private const int VSpaceNameIndexId = 2;

        private const int VIndexSpaceId = 289;
        private const int VIndexNameIndexId = 2;

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

        public async Task<IList<MessagePackObject>> FindSpaceByNameAsync(string spaceName)
        {
            var selectResult = await _connectionPool.RequestAsync(new SelectRequest
            {
                SpaceId = VSpaceSpaceId,
                IndexId = VSpaceNameIndexId,
                Iterator = Iterator.Eq,
                Key = new List<object> {spaceName}
            });
            return selectResult.Count == 0 ? null : selectResult.First().AsList();
        }

        public async Task<IList<MessagePackObject>> FindIndexByNameAsync(uint spaceId, string indexName)
        {
            var selectResult = await _connectionPool.RequestAsync(new SelectRequest
            {
                SpaceId = VIndexSpaceId,
                IndexId = VIndexNameIndexId,
                Iterator = Iterator.Eq,
                Key = new List<object> {spaceId, indexName}
            });
            return selectResult.Count == 0 ? null : selectResult.First().AsList();
        }
    }
}