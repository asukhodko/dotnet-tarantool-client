using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    public class TarantoolSpace<T> : ITarantoolSpace<T>
    {
        private readonly Dictionary<string, Index> _indexes = new Dictionary<string, Index>();
        private readonly string _spaceName;

        public TarantoolSpace(ITarantoolClient tarantoolClient, uint spaceId)
        {
            TarantoolClient = tarantoolClient;
            SpaceId = spaceId;
        }

        public TarantoolSpace(ITarantoolClient tarantoolClient, string spaceName)
        {
            TarantoolClient = tarantoolClient;
            _spaceName = spaceName;
        }

        public uint SpaceId { get; private set; }

        public ITarantoolClient TarantoolClient { get; }

        public ITarantoolIndex<T, TK> GetIndex<TK>(uint indexId)
        {
            return new TarantoolIndex<T, TK>(TarantoolClient, this, indexId);
        }

        public ITarantoolIndex<T, TK> GetIndex<TK>(string indexName)
        {
            return new TarantoolIndex<T, TK>(TarantoolClient, this, indexName);
        }

        public async Task<IList<T>> InsertAsync(T entity, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            var result = await TarantoolClient.InsertAsync(new InsertRequest<T>
            {
                SpaceId = SpaceId,
                Tuple = entity
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> UpdateAsync(IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            var result = await TarantoolClient.UpdateAsync<T>(new UpdateRequest
            {
                SpaceId = SpaceId,
                Key = key,
                UpdateOperations = updateOperations
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> UpdateAsync(uint indexId, IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            var result = await TarantoolClient.UpdateAsync<T>(new UpdateRequest
            {
                SpaceId = SpaceId,
                IndexId = indexId,
                Key = key,
                UpdateOperations = updateOperations
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> UpdateAsync(string indexName, IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            var index = await FindIndexByNameAsync(indexName, cancellationToken);
            var result = await TarantoolClient.UpdateAsync<T>(new UpdateRequest
            {
                SpaceId = SpaceId,
                IndexId = index.IndexId,
                Key = key,
                UpdateOperations = updateOperations
            }, cancellationToken);
            return result;
        }

        public async Task<IList<MessagePackObject>> DeleteAsync(IEnumerable<object> key,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            var result = await TarantoolClient.DeleteAsync(new DeleteRequest
            {
                SpaceId = SpaceId,
                Key = key
            }, cancellationToken);
            return result;
        }

        public async Task<IList<MessagePackObject>> DeleteAsync(uint indexId, IEnumerable<object> key,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            var result = await TarantoolClient.DeleteAsync(new DeleteRequest
            {
                SpaceId = SpaceId,
                IndexId = indexId,
                Key = key
            }, cancellationToken);
            return result;
        }

        public async Task<IList<MessagePackObject>> DeleteAsync(string indexName, IEnumerable<object> key,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            var index = await FindIndexByNameAsync(indexName, cancellationToken);
            var result = await TarantoolClient.DeleteAsync(new DeleteRequest
            {
                SpaceId = SpaceId,
                IndexId = index.IndexId,
                Key = key
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> ReplaceAsync(T entity, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            var result = await TarantoolClient.ReplaceAsync(new ReplaceRequest<T>
            {
                SpaceId = SpaceId,
                Tuple = entity
            }, cancellationToken);
            return result;
        }

        public async Task UpsertAsync(T entity, IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            await TarantoolClient.UpsertAsync(new UpsertRequest<T>
            {
                SpaceId = SpaceId,
                Tuple = entity,
                UpdateOperations = updateOperations
            }, cancellationToken);
        }

        public async Task<Index> FindIndexByNameAsync(string indexName, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken);
            if (!_indexes.ContainsKey(indexName))
                _indexes[indexName] =
                    await TarantoolClient.FindIndexByNameAsync(SpaceId, indexName, cancellationToken);
            return _indexes[indexName];
        }

        public async Task EnsureHaveSpaceIdAsync(CancellationToken cancellationToken)
        {
            if (SpaceId != 0)
                return;
            SpaceId = (await TarantoolClient.FindSpaceByNameAsync(_spaceName, cancellationToken)).SpaceId;
        }
    }
}