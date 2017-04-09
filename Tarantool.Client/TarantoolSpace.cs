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
        private uint _spaceId;

        public TarantoolSpace(ITarantoolClient tarantoolClient, uint spaceId)
        {
            TarantoolClient = tarantoolClient;
            _spaceId = spaceId;
        }

        public TarantoolSpace(ITarantoolClient tarantoolClient, string spaceName)
        {
            TarantoolClient = tarantoolClient;
            _spaceName = spaceName;
        }

        public ITarantoolClient TarantoolClient { get; }

        public async Task<IList<T>> SelectAsync(IEnumerable<object> key, Iterator iterator, uint offset, uint limit,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var result = await TarantoolClient.SelectAsync<T>(new SelectRequest
            {
                SpaceId = _spaceId,
                Key = key,
                Iterator = iterator,
                Offset = offset,
                Limit = limit
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> SelectAsync(uint indexId, IEnumerable<object> key, Iterator iterator, uint offset,
            uint limit, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var result = await TarantoolClient.SelectAsync<T>(new SelectRequest
            {
                SpaceId = _spaceId,
                IndexId = indexId,
                Key = key,
                Iterator = iterator,
                Offset = offset,
                Limit = limit
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> SelectAsync(string indexName, IEnumerable<object> key, Iterator iterator,
            uint offset, uint limit, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var index = await FindIndexByNameAsync(indexName, cancellationToken);
            var result = await TarantoolClient.SelectAsync<T>(new SelectRequest
            {
                SpaceId = _spaceId,
                IndexId = index.IndexId,
                Key = key,
                Iterator = iterator,
                Offset = offset,
                Limit = limit
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> InsertAsync(T entity, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var result = await TarantoolClient.InsertAsync(new InsertRequest<T>
            {
                SpaceId = _spaceId,
                Tuple = entity
            }, cancellationToken);
            return result;
        }

        public async Task<IList<MessagePackObject>> UpdateAsync(IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var result = await TarantoolClient.UpdateAsync(new UpdateRequest
            {
                SpaceId = _spaceId,
                Key = key,
                UpdateOperations = updateOperations
            }, cancellationToken);
            return result;
        }

        public async Task<IList<MessagePackObject>> UpdateAsync(uint indexId, IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var result = await TarantoolClient.UpdateAsync(new UpdateRequest
            {
                SpaceId = _spaceId,
                IndexId = indexId,
                Key = key,
                UpdateOperations = updateOperations
            }, cancellationToken);
            return result;
        }

        public async Task<IList<MessagePackObject>> UpdateAsync(string indexName, IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var index = await FindIndexByNameAsync(indexName, cancellationToken);
            var result = await TarantoolClient.UpdateAsync(new UpdateRequest
            {
                SpaceId = _spaceId,
                IndexId = index.IndexId,
                Key = key,
                UpdateOperations = updateOperations
            }, cancellationToken);
            return result;
        }

        public async Task<IList<MessagePackObject>> DeleteAsync(IEnumerable<object> key,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var result = await TarantoolClient.DeleteAsync(new DeleteRequest
            {
                SpaceId = _spaceId,
                Key = key
            }, cancellationToken);
            return result;
        }

        public async Task<IList<MessagePackObject>> DeleteAsync(uint indexId, IEnumerable<object> key,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var result = await TarantoolClient.DeleteAsync(new DeleteRequest
            {
                SpaceId = _spaceId,
                IndexId = indexId,
                Key = key
            }, cancellationToken);
            return result;
        }

        public async Task<IList<MessagePackObject>> DeleteAsync(string indexName, IEnumerable<object> key,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var index = await FindIndexByNameAsync(indexName, cancellationToken);
            var result = await TarantoolClient.DeleteAsync(new DeleteRequest
            {
                SpaceId = _spaceId,
                IndexId = index.IndexId,
                Key = key
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> ReplaceAsync(T entity, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            var result = await TarantoolClient.ReplaceAsync(new ReplaceRequest<T>
            {
                SpaceId = _spaceId,
                Tuple = entity
            }, cancellationToken);
            return result;
        }

        public async Task UpsertAsync(T entity, IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            await TarantoolClient.UpsertAsync(new UpsertRequest<T>
            {
                SpaceId = _spaceId,
                Tuple = entity,
                UpdateOperations = updateOperations
            }, cancellationToken);
        }

        public async Task<Index> FindIndexByNameAsync(string indexName, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceId(cancellationToken);
            if (!_indexes.ContainsKey(indexName))
                _indexes[indexName] =
                    await TarantoolClient.FindIndexByNameAsync(_spaceId, indexName, cancellationToken);
            return _indexes[indexName];
        }

        private async Task EnsureHaveSpaceId(CancellationToken cancellationToken)
        {
            if (_spaceId != 0)
                return;
            _spaceId = (await TarantoolClient.FindSpaceByNameAsync(_spaceName, cancellationToken)).SpaceId;
        }
    }
}