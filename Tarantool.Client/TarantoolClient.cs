using System;
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

        public async Task<IList<MessagePackObject>> InsertAsync(InsertRequest insertRequest)
        {
            return (await RequestAsync(insertRequest)).AsList();
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

        public async Task<IList<MessagePackObject>> FindSpaceByNameAsync(string spaceName)
        {
            var selectResult = (await _connectionPool.RequestAsync(new SelectRequest
            {
                SpaceId = VSpaceSpaceId,
                IndexId = VSpaceNameIndexId,
                Iterator = Iterator.Eq,
                Key = new List<object> { spaceName }
            })).AsList();
            return selectResult.Count == 0 ? null : selectResult.First().AsList();
        }

        public async Task<IList<MessagePackObject>> FindIndexByNameAsync(uint spaceId, string indexName)
        {
            var selectResult = (await _connectionPool.RequestAsync(new SelectRequest
            {
                SpaceId = VIndexSpaceId,
                IndexId = VIndexNameIndexId,
                Iterator = Iterator.Eq,
                Key = new List<object> { spaceId, indexName }
            })).AsList();
            return selectResult.Count == 0 ? null : selectResult.First().AsList();
        }

        /*public async Task<T> RequestAsync<T>(ClientMessageBase clientMessage) where T : class, new()
        {
            var mapper = new MessagePackObjectMapper<T>();
            var result = await _connectionPool.RequestAsync(clientMessage);
            return result.Select(x =>
            {
                var t = new T();
                mapper.Map(x, t);
                return t;
            }).ToList();
        }*/
    }
}