using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    /// <summary>The Tarantool space indexes accessor class.</summary>
    /// <typeparam name="T">The class for object mapping.</typeparam>
    /// <typeparam name="TKey">The <see cref="IndexKey" /> type.</typeparam>
    public class TarantoolIndex<T, TKey> : ITarantoolIndex<T, TKey>
        where TKey : IndexKey
    {
        private readonly string _indexName;

        /// <summary>Initializes a new instance of the <see cref="TarantoolIndex{T, TKey}" /> class by index id.</summary>
        /// <param name="tarantoolClient">The tarantool client.</param>
        /// <param name="space">The space.</param>
        /// <param name="indexId">The index id.</param>
        public TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, uint indexId)
        {
            TarantoolClient = tarantoolClient;
            Space = space;
            IndexId = indexId;
        }

        /// <summary>Initializes a new instance of the <see cref="TarantoolIndex{T, TKey}" /> class by index name.</summary>
        /// <param name="tarantoolClient">The tarantool client.</param>
        /// <param name="space">The space.</param>
        /// <param name="indexName">The index name.</param>
        public TarantoolIndex(ITarantoolClient tarantoolClient, ITarantoolSpace<T> space, string indexName)
        {
            TarantoolClient = tarantoolClient;
            Space = space;
            _indexName = indexName;
        }

        /// <summary>Gets the index id. Return null if id not have yet (see <see cref="EnsureHaveIndexIdAsync" />).</summary>
        public uint? IndexId { get; private set; }

        private ITarantoolSpace<T> Space { get; }

        private ITarantoolClient TarantoolClient { get; }

        /// <summary>Delete from space by key.</summary>
        /// <param name="key">The key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IList<T>> DeleteAsync(TKey key, CancellationToken cancellationToken)
        {
            return await (await DeleteAsyncAsync(key, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <summary>Delete from space by key.</summary>
        /// <param name="key">The key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<Task<IList<T>>> DeleteAsyncAsync(TKey key, CancellationToken cancellationToken)
        {
            await EnsureHaveIndexIdAsync(cancellationToken).ConfigureAwait(false);
            Debug.Assert(IndexId != null, "IndexId != null");
            return await TarantoolClient.RequestAsyncAsync<T>(
                           new DeleteRequest { SpaceId = Space.SpaceId, IndexId = IndexId.Value, Key = key.Key },
                           cancellationToken)
                       .ConfigureAwait(false);
        }

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
            return await (await SelectAsyncAsync(iterator, offset, limit, cancellationToken).ConfigureAwait(false))
                       .ConfigureAwait(false);
        }

        /// <summary>Select from space by key</summary>
        /// <param name="key">The key filed (array or list).</param>
        /// <param name="iterator">The iterator.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IList<T>> SelectAsync(
            TKey key,
            Iterator iterator,
            uint offset,
            uint limit,
            CancellationToken cancellationToken)
        {
            return await (await SelectAsyncAsync(key, iterator, offset, limit, cancellationToken).ConfigureAwait(false))
                       .ConfigureAwait(false);
        }

        /// <summary>Select all records from space</summary>
        /// <param name="iterator">The iterator.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<Task<IList<T>>> SelectAsyncAsync(
            Iterator iterator,
            uint offset,
            uint limit,
            CancellationToken cancellationToken)
        {
            await EnsureHaveIndexIdAsync(cancellationToken).ConfigureAwait(false);
            Debug.Assert(IndexId != null, "IndexId != null");
            return await TarantoolClient.RequestAsyncAsync<T>(
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
        }

        /// <summary>Select from space by key</summary>
        /// <param name="key">The key filed (array or list).</param>
        /// <param name="iterator">The iterator.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<Task<IList<T>>> SelectAsyncAsync(
            TKey key,
            Iterator iterator,
            uint offset,
            uint limit,
            CancellationToken cancellationToken)
        {
            await EnsureHaveIndexIdAsync(cancellationToken).ConfigureAwait(false);
            Debug.Assert(IndexId != null, "IndexId != null");
            return await TarantoolClient.RequestAsyncAsync<T>(
                           new SelectRequest
                           {
                               SpaceId = Space.SpaceId,
                               IndexId = IndexId.Value,
                               Key = key.Key,
                               Iterator = iterator,
                               Offset = offset,
                               Limit = limit
                           },
                           cancellationToken)
                       .ConfigureAwait(false);
        }

        /// <summary>Performs an updates in space.</summary>
        /// <param name="key">The key.</param>
        /// <param name="updateDefinition">The update operations list.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with replaced data as result.</returns>
        public async Task<IList<T>> UpdateAsync(
            TKey key,
            UpdateDefinition<T> updateDefinition,
            CancellationToken cancellationToken)
        {
            return await (await UpdateAsyncAsync(key, updateDefinition, cancellationToken).ConfigureAwait(false))
                       .ConfigureAwait(false);
        }

        /// <summary>Performs an updates in space.</summary>
        /// <param name="key">The key.</param>
        /// <param name="updateDefinition">The update operations list.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with replaced data as result.</returns>
        public async Task<Task<IList<T>>> UpdateAsyncAsync(
            TKey key,
            UpdateDefinition<T> updateDefinition,
            CancellationToken cancellationToken)
        {
            await EnsureHaveIndexIdAsync(cancellationToken).ConfigureAwait(false);
            Debug.Assert(IndexId != null, "IndexId != null");
            return await TarantoolClient.RequestAsyncAsync<T>(
                           new UpdateRequest
                           {
                               SpaceId = Space.SpaceId,
                               IndexId = IndexId.Value,
                               Key = key.Key,
                               UpdateOperations = updateDefinition.UpdateOperations
                           },
                           cancellationToken)
                       .ConfigureAwait(false);
        }
    }
}