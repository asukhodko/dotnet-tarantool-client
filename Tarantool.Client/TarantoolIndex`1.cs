using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Tarantool.Client.Models;

namespace Tarantool.Client
{
    /// <summary>
    /// The tarantool index accessor for indexes of one filed.</summary>
    /// <typeparam name="T">The class for object mapping.</typeparam>
    /// <typeparam name="TK">The type for key mapping</typeparam>
    public class TarantoolIndex<T, TK> : TarantoolIndex<T>, ITarantoolIndex<T, TK>
    {
        /// <summary>Initializes a new instance of the <see cref="TarantoolIndex{T, TK}" /> class by indexId.</summary>
        /// <param name="tarantoolClient">The tarantool client.</param>
        /// <param name="space">The space.</param>
        /// <param name="indexId">The index id.</param>
        public TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, uint indexId)
            : base(tarantoolClient, space, indexId)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TarantoolIndex{T, TK}" /> class by indexName.</summary>
        /// <param name="tarantoolClient">The tarantool client.</param>
        /// <param name="space">The space.</param>
        /// <param name="indexName">The index name.</param>
        public TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, string indexName)
            : base(tarantoolClient, space, indexName)
        {
        }

        /// <summary>Select from space by key</summary>
        /// <param name="key">The key value.</param>
        /// <param name="iterator">The iterator.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>The <see cref="Task" />.</returns>
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