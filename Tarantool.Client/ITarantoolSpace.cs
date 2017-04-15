using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MsgPack;

using Tarantool.Client.Models;

namespace Tarantool.Client
{
    /// <summary>The interface to Tarantool spaces.</summary>
    /// <typeparam name="T">The class for object mapping.</typeparam>
    public interface ITarantoolSpace<T>
    {
        /// <summary>Gets the space id. Returns 0 if id not have yet (see <see cref="EnsureHaveSpaceIdAsync" />).</summary>
        uint SpaceId { get; }

        /// <summary>Ensures have space id. If not then retrieves it by name. </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        Task EnsureHaveSpaceIdAsync(CancellationToken cancellationToken);

        Task<Index> FindIndexByNameAsync(
            string indexName,
            CancellationToken cancellationToken = default(CancellationToken));

        ITarantoolIndex<T, TKey> GetIndex<TKey>(uint indexId)
            where TKey : IndexKey;

        ITarantoolIndex<T, TKey> GetIndex<TKey>(string indexName)
            where TKey : IndexKey;

        Task<IList<T>> InsertAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> ReplaceAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> UpdateAsync(
            IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> UpdateAsync(
            uint indexId,
            IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> UpdateAsync(
            string indexName,
            IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken = default(CancellationToken));

        Task UpsertAsync(
            T entity,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}