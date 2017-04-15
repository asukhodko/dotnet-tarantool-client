using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>Return an instance of the <see cref="ITarantoolIndex{T, TKey}" /> interface by index id.</summary>
        /// <param name="indexId">The index id.</param>
        /// <typeparam name="TKey">The <see cref="IndexKey" /> type.</typeparam>
        /// <returns>The <see cref="ITarantoolIndex{T, TKey}" />.</returns>
        ITarantoolIndex<T, TKey> GetIndex<TKey>(uint indexId)
            where TKey : IndexKey;

        /// <summary>Return an instance of the <see cref="ITarantoolIndex{T, TKey}" /> interface by index name.</summary>
        /// <param name="indexName">The index name.</param>
        /// <typeparam name="TKey">The <see cref="IndexKey" /> type.</typeparam>
        /// <returns>The <see cref="ITarantoolIndex{T, TKey}" />.</returns>
        ITarantoolIndex<T, TKey> GetIndex<TKey>(string indexName)
            where TKey : IndexKey;

        /// <summary>Inserts entity into space.</summary>
        /// <param name="entity">The entity for insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with inserted data as result.</returns>
        Task<IList<T>> InsertAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Replaces entity in space.</summary>
        /// <param name="entity">The entity for replace.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with replaced data as result.</returns>
        Task<IList<T>> ReplaceAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task UpsertAsync(
            T entity,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}