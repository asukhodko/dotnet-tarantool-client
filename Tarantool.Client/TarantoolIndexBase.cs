using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    public class TarantoolIndexBase<T>
    {
        private readonly string _indexName;

        protected TarantoolIndexBase(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, uint indexId)
        {
            TarantoolClient = tarantoolClient;
            Space = space;
            IndexId = indexId;
        }

        protected TarantoolIndexBase(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, string indexName)
        {
            TarantoolClient = tarantoolClient;
            Space = space;
            _indexName = indexName;
        }

        private ITarantoolSpace<T> Space { get; }
        public uint? IndexId { get; private set; }
        private ITarantoolClient TarantoolClient { get; }

        protected async Task<IList<T>> SelectAsync(IEnumerable<object> key, Iterator iterator, uint offset,
            uint limit, CancellationToken cancellationToken)
        {
            await EnsureHaveIndexIdAsync(cancellationToken);
            Debug.Assert(IndexId != null);
            var result = await TarantoolClient.SelectAsync<T>(new SelectRequest
            {
                SpaceId = Space.SpaceId,
                IndexId = IndexId.Value,
                Key = key,
                Iterator = iterator,
                Offset = offset,
                Limit = limit
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> SelectAsync(Iterator iterator, uint offset,
            uint limit, CancellationToken cancellationToken)
        {
            await EnsureHaveIndexIdAsync(cancellationToken);
            Debug.Assert(IndexId != null);
            var result = await TarantoolClient.SelectAsync<T>(new SelectRequest
            {
                SpaceId = Space.SpaceId,
                IndexId = IndexId.Value,
                Iterator = iterator,
                Offset = offset,
                Limit = limit
            }, cancellationToken);
            return result;
        }

        public async Task EnsureHaveIndexIdAsync(CancellationToken cancellationToken)
        {
            await Space.EnsureHaveSpaceIdAsync(cancellationToken);
            if (IndexId != null)
                return;
            IndexId = (await TarantoolClient.FindIndexByNameAsync(Space.SpaceId, _indexName, cancellationToken))
                .IndexId;
        }
    }
}