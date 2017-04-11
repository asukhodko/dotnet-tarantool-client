using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    public class TarantoolIndex<T, TK> : ITarantoolIndex<T, TK>
    {
        private readonly string _indexName;

        public TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, uint indexId)
        {
            TarantoolClient = tarantoolClient;
            Space = space;
            IndexId = indexId;
        }

        public TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, string indexName)
        {
            TarantoolClient = tarantoolClient;
            Space = space;
            _indexName = indexName;
        }

        public ITarantoolSpace<T> Space { get; }
        public uint? IndexId { get; private set; }
        public ITarantoolClient TarantoolClient { get; }

        public async Task<IList<T>> SelectAsync(TK key, Iterator iterator, uint offset,
            uint limit, CancellationToken cancellationToken)
        {
            await EnsureHaveIndexId(cancellationToken);
            Debug.Assert(IndexId != null);
            var result = await TarantoolClient.SelectAsync<T>(new SelectRequest
            {
                SpaceId = Space.SpaceId,
                IndexId = IndexId.Value,
                Key = new object[] { key },
                Iterator = iterator,
                Offset = offset,
                Limit = limit
            }, cancellationToken);
            return result;
        }

        public async Task<IList<T>> SelectAsync(Iterator iterator, uint offset,
            uint limit, CancellationToken cancellationToken)
        {
            await EnsureHaveIndexId(cancellationToken);
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

        private async Task EnsureHaveIndexId(CancellationToken cancellationToken)
        {
            await Space.EnsureHaveSpaceIdAsync(cancellationToken);
            if (IndexId != null)
                return;
            IndexId = (await TarantoolClient.FindIndexByNameAsync(Space.SpaceId, _indexName, cancellationToken))
                .IndexId;
        }
    }
}