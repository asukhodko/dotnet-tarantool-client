using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    /// <summary>The base class to Tarantool space indexes.</summary>
    /// <typeparam name="T">The class for object mapping.</typeparam>
    public abstract class TarantoolIndex<T> : ITarantoolIndex<T>
    {
        private readonly string _indexName;

        /// <summary>Initializes a new instance of the <see cref="TarantoolIndex{T}" /> class by indexId.</summary>
        /// <param name="tarantoolClient">The tarantool client.</param>
        /// <param name="space">The space.</param>
        /// <param name="indexId">The index id.</param>
        protected TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, uint indexId)
        {
            TarantoolClient = tarantoolClient;
            Space = space;
            IndexId = indexId;
        }

        /// <summary>Initializes a new instance of the <see cref="TarantoolIndex{T}" /> class by indexName.</summary>
        /// <param name="tarantoolClient">The tarantool client.</param>
        /// <param name="space">The space.</param>
        /// <param name="indexName">The index name.</param>
        protected TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, string indexName)
        {
            TarantoolClient = tarantoolClient;
            Space = space;
            _indexName = indexName;
        }

        /// <summary>Gets the index id. Return null if id not have yet (see <see cref="EnsureHaveIndexIdAsync" />).</summary>
        public uint? IndexId { get; private set; }

        private ITarantoolSpace<T> Space { get; }

        private ITarantoolClient TarantoolClient { get; }

        /// <summary>Ensures have index id. If not then retrieves it by name. </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task EnsureHaveIndexIdAsync(CancellationToken cancellationToken)
        {
            await Space.EnsureHaveSpaceIdAsync(cancellationToken).ConfigureAwait(false);
            if (IndexId != null) return;
            IndexId = (await TarantoolClient.FindIndexByNameAsync(Space.SpaceId, _indexName, cancellationToken)
                           .ConfigureAwait(false)).IndexId;
        }

        /// <summary>Select all records from space</summary>
        /// <param name="iterator">The iterator.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IList<T>> SelectAsync(
            Iterator iterator,
            uint offset,
            uint limit,
            CancellationToken cancellationToken)
        {
            await EnsureHaveIndexIdAsync(cancellationToken).ConfigureAwait(false);
            Debug.Assert(IndexId != null, "IndexId != null");
            var result = await TarantoolClient.SelectAsync<T>(
                                 new SelectRequest
                                 {
                                     SpaceId = Space.SpaceId,
                                     IndexId = IndexId.Value,
                                     Iterator = iterator,
                                     Offset = offset,
                                     Limit = limit
                                 },
                                 cancellationToken)
                             .ConfigureAwait(false);
            return result;
        }

        /// <summary>Select from space by key</summary>
        /// <param name="key">The key filed (array or list).</param>
        /// <param name="iterator">The iterator.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        protected async Task<IList<T>> SelectAsync(
            IEnumerable<object> key,
            Iterator iterator,
            uint offset,
            uint limit,
            CancellationToken cancellationToken)
        {
            await EnsureHaveIndexIdAsync(cancellationToken).ConfigureAwait(false);
            Debug.Assert(IndexId != null, "IndexId != null");
            var result = await TarantoolClient.SelectAsync<T>(
                                 new SelectRequest
                                 {
                                     SpaceId = Space.SpaceId,
                                     IndexId = IndexId.Value,
                                     Key = key,
                                     Iterator = iterator,
                                     Offset = offset,
                                     Limit = limit
                                 },
                                 cancellationToken)
                             .ConfigureAwait(false);
            return result;
        }
    }
}