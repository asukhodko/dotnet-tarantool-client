using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Tarantool.Client.Models;

namespace Tarantool.Client
{
    public class TarantoolIndex<T, TK> : TarantoolIndexBase<T>, ITarantoolIndex<T, TK>
    {
        public TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, uint indexId)
            : base(tarantoolClient, space, indexId)
        {
        }

        public TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, string indexName)
            : base(tarantoolClient, space, indexName)
        {
        }

        public async Task<IList<T>> SelectAsync(
            TK key,
            Iterator iterator,
            uint offset,
            uint limit,
            CancellationToken cancellationToken)
        {
            var result = await SelectAsync(new object[] { key }, iterator, offset, limit, cancellationToken)
                             .ConfigureAwait(false);
            return result;
        }
    }
}