using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>Gets the space id. Returns 0 if id not have yet (see <see cref="EnsureHaveSpaceIdAsync" />).</summary>
        public uint SpaceId { get; private set; }

        private ITarantoolClient TarantoolClient { get; }

        /// <summary>Ensures have space id. If not then retrieves it by name. </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task EnsureHaveSpaceIdAsync(CancellationToken cancellationToken)
        {
            if (SpaceId != 0) return;
            SpaceId = (await TarantoolClient.FindSpaceByNameAsync(_spaceName, cancellationToken).ConfigureAwait(false))
                .SpaceId;
        }

        /// <summary>Return an instance of the <see cref="ITarantoolIndex{T, TKey}" /> interface by index id.</summary>
        /// <param name="indexId">The index id.</param>
        /// <typeparam name="TKey">The <see cref="IndexKey" /> type.</typeparam>
        /// <returns>The <see cref="ITarantoolIndex{T, TKey}" />.</returns>
        public ITarantoolIndex<T, TKey> GetIndex<TKey>(uint indexId)
            where TKey : IndexKey
        {
            return new TarantoolIndex<T, TKey>(TarantoolClient, this, indexId);
        }

        /// <summary>Return an instance of the <see cref="ITarantoolIndex{T, TKey}" /> interface by index name.</summary>
        /// <param name="indexName">The index name.</param>
        /// <typeparam name="TKey">The <see cref="IndexKey" /> type.</typeparam>
        /// <returns>The <see cref="ITarantoolIndex{T, TKey}" />.</returns>
        public ITarantoolIndex<T, TKey> GetIndex<TKey>(string indexName)
            where TKey : IndexKey
        {
            return new TarantoolIndex<T, TKey>(TarantoolClient, this, indexName);
        }

        /// <summary>Inserts entity into space.</summary>
        /// <param name="entity">The entity for insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with inserted data as result.</returns>
        public async Task<IList<T>> InsertAsync(T entity, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken).ConfigureAwait(false);
            var result = await TarantoolClient.InsertAsync(
                                 new InsertRequest<T> { SpaceId = SpaceId, Tuple = entity },
                                 cancellationToken)
                             .ConfigureAwait(false);
            return result;
        }

        /// <summary>Replaces entity in space.</summary>
        /// <param name="entity">The entity for replace.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with replaced data as result.</returns>
        public async Task<IList<T>> ReplaceAsync(T entity, CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken).ConfigureAwait(false);
            var result = await TarantoolClient.ReplaceAsync(
                                 new ReplaceRequest<T> { SpaceId = SpaceId, Tuple = entity },
                                 cancellationToken)
                             .ConfigureAwait(false);
            return result;
        }

        /// <summary>Searches entity by primary key and updates it if found or inserts it if not found.</summary>
        /// <param name="entity">The entity for replace.</param>
        /// <param name="updateOperations">The update operations list.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> for awaiting completion.</returns>
        public async Task UpsertAsync(
            T entity,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken)
        {
            await EnsureHaveSpaceIdAsync(cancellationToken).ConfigureAwait(false);
            await TarantoolClient.UpsertAsync(
                    new UpsertRequest<T> { SpaceId = SpaceId, Tuple = entity, UpdateOperations = updateOperations },
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}