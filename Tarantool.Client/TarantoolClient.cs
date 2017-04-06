using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
using Tarantool.Client.Serialization;

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

        public async Task<MessagePackObject> RequestAsync(ClientMessageBase clientMessage)
        {
            return await _connectionPool.RequestAsync(clientMessage);
        }

        public async Task<IList<MessagePackObject>> SelectAsync(SelectRequest selectRequest)
        {
            return (await RequestAsync(selectRequest)).AsList();
        }

        public async Task<IList<T>> SelectAsync<T>(SelectRequest selectRequest)
        {
            var result = await SelectAsync(selectRequest);
            return MapCollection<T>(result).ToList();
        }

        public async Task<IList<MessagePackObject>> InsertAsync(InsertRequest insertRequest)
        {
            return (await RequestAsync(insertRequest)).AsList();
        }

        public async Task<IList<T>> InsertAsync<T>(InsertRequest<T> insertRequest)
        {
            var result = (await RequestAsync(insertRequest)).AsList();
            return MapCollection<T>(result).ToList();
        }

        public async Task<IList<MessagePackObject>> UpdateAsync(UpdateRequest updateRequest)
        {
            return (await RequestAsync(updateRequest)).AsList();
        }

        public async Task<IList<MessagePackObject>> DeleteAsync(DeleteRequest deleteRequest)
        {
            return (await RequestAsync(deleteRequest)).AsList();
        }

        public async Task<IList<MessagePackObject>> ReplaceAsync(ReplaceRequest replaceRequest)
        {
            return (await RequestAsync(replaceRequest)).AsList();
        }

        public async Task UpsertAsync(UpsertRequest upsertRequest)
        {
            await RequestAsync(upsertRequest);
        }

        public async Task<MessagePackObject> CallAsync(CallRequest callRequest)
        {
            return await RequestAsync(callRequest);
        }

        public async Task<MessagePackObject> EvalAsync(EvalRequest evalRequest)
        {
            return await RequestAsync(evalRequest);
        }

        public async Task<Space> FindSpaceByNameAsync(string spaceName)
        {
            var selectResult = await SelectAsync<Space>(new SelectRequest
            {
                SpaceId = VSpaceSpaceId,
                IndexId = VSpaceNameIndexId,
                Iterator = Iterator.Eq,
                Key = new List<object> { spaceName }
            });
            return selectResult.FirstOrDefault();
        }

        public async Task<Index> FindIndexByNameAsync(uint spaceId, string indexName)
        {
            var selectResult = await SelectAsync<Index>(new SelectRequest
            {
                SpaceId = VIndexSpaceId,
                IndexId = VIndexNameIndexId,
                Iterator = Iterator.Eq,
                Key = new List<object> { spaceId, indexName }
            });
            return selectResult.FirstOrDefault();
        }

        private IEnumerable<T> MapCollection<T>(IEnumerable<MessagePackObject> source)
        {
            return source.Select(x =>
            {
                var t = MessagePackObjectMapper.Map<T>(x);
                return t;
            });
        }
    }
}