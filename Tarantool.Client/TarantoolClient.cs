using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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

        private TarantoolClient(ConnectionOptions connectionOptions)
        {
            _connectionPool = ConnectionPool.GetPool(connectionOptions);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await _connectionPool.ConnectAsync(cancellationToken);
        }

        public async Task<MessagePackObject> RequestAsync(ClientMessageBase clientMessage,
            CancellationToken cancellationToken)
        {
            return await _connectionPool.RequestAsync(clientMessage, cancellationToken);
        }

        public async Task<IList<MessagePackObject>> SelectAsync(SelectRequest selectRequest,
            CancellationToken cancellationToken)
        {
            return (await RequestAsync(selectRequest, cancellationToken)).AsList();
        }

        public async Task<IList<T>> SelectAsync<T>(SelectRequest selectRequest, CancellationToken cancellationToken)
        {
            var result = await SelectAsync(selectRequest, cancellationToken);
            return MapCollection<T>(result).ToList();
        }

        public async Task<IList<MessagePackObject>> InsertAsync(InsertRequest insertRequest,
            CancellationToken cancellationToken)
        {
            return (await RequestAsync(insertRequest, cancellationToken)).AsList();
        }

        public async Task<IList<T>> InsertAsync<T>(InsertRequest<T> insertRequest, CancellationToken cancellationToken)
        {
            var result = (await RequestAsync(insertRequest, cancellationToken)).AsList();
            return MapCollection<T>(result).ToList();
        }

        public async Task<IList<MessagePackObject>> UpdateAsync(UpdateRequest updateRequest,
            CancellationToken cancellationToken)
        {
            return (await RequestAsync(updateRequest, cancellationToken)).AsList();
        }

        public async Task<IList<MessagePackObject>> DeleteAsync(DeleteRequest deleteRequest,
            CancellationToken cancellationToken)
        {
            return (await RequestAsync(deleteRequest, cancellationToken)).AsList();
        }

        public async Task<IList<MessagePackObject>> ReplaceAsync(ReplaceRequest replaceRequest,
            CancellationToken cancellationToken)
        {
            return (await RequestAsync(replaceRequest, cancellationToken)).AsList();
        }

        public async Task<IList<T>> ReplaceAsync<T>(ReplaceRequest<T> replaceRequest,
            CancellationToken cancellationToken)
        {
            var result = (await RequestAsync(replaceRequest, cancellationToken)).AsList();
            return MapCollection<T>(result).ToList();
        }

        public async Task UpsertAsync(UpsertRequest upsertRequest, CancellationToken cancellationToken)
        {
            await RequestAsync(upsertRequest, cancellationToken);
        }

        public async Task UpsertAsync<T>(UpsertRequest<T> upsertRequest, CancellationToken cancellationToken)
        {
            await RequestAsync(upsertRequest, cancellationToken);
        }

        public async Task<MessagePackObject> CallAsync(CallRequest callRequest, CancellationToken cancellationToken)
        {
            return await RequestAsync(callRequest, cancellationToken);
        }

        public async Task<MessagePackObject> EvalAsync(EvalRequest evalRequest, CancellationToken cancellationToken)
        {
            return await RequestAsync(evalRequest, cancellationToken);
        }

        public async Task<Space> FindSpaceByNameAsync(string spaceName, CancellationToken cancellationToken)
        {
            var selectResult = await SelectAsync<Space>(new SelectRequest
            {
                SpaceId = VSpaceSpaceId,
                IndexId = VSpaceNameIndexId,
                Iterator = Iterator.Eq,
                Key = new List<object> { spaceName }
            }, cancellationToken);
            return selectResult.FirstOrDefault();
        }

        public async Task<Index> FindIndexByNameAsync(uint spaceId, string indexName,
            CancellationToken cancellationToken)
        {
            var selectResult = await SelectAsync<Index>(new SelectRequest
            {
                SpaceId = VIndexSpaceId,
                IndexId = VIndexNameIndexId,
                Iterator = Iterator.Eq,
                Key = new List<object> { spaceId, indexName }
            }, cancellationToken);
            return selectResult.FirstOrDefault();
        }

        public ITarantoolSpace<T> GetSpace<T>(uint spaceId)
        {
            return new TarantoolSpace<T>(this, spaceId);
        }

        public ITarantoolSpace<T> GetSpace<T>(string spaceName)
        {
            return new TarantoolSpace<T>(this, spaceName);
        }

        public static ITarantoolClient Create(ConnectionOptions connectionOptions)
        {
            return new TarantoolClient(connectionOptions);
        }

        public static ITarantoolClient Create(string connectionString)
        {
            return new TarantoolClient(new ConnectionOptions(connectionString));
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