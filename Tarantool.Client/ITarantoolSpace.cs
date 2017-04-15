using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MsgPack;

using Tarantool.Client.Models;

namespace Tarantool.Client
{
    public interface ITarantoolSpace<T>
    {
        uint SpaceId { get; }

        Task<IList<MessagePackObject>> DeleteAsync(
            IEnumerable<object> key,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> DeleteAsync(
            uint indexId,
            IEnumerable<object> key,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> DeleteAsync(
            string indexName,
            IEnumerable<object> key,
            CancellationToken cancellationToken = default(CancellationToken));

        Task EnsureHaveSpaceIdAsync(CancellationToken cancellationToken);

        Task<Index> FindIndexByNameAsync(
            string indexName,
            CancellationToken cancellationToken = default(CancellationToken));

        ITarantoolIndex<T, TKey> GetIndex<TKey>(uint indexId) where TKey : IndexKey;

        ITarantoolIndex<T, TKey> GetIndex<TKey>(string indexName) where TKey : IndexKey;

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