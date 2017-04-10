using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models;

namespace Tarantool.Client
{
    public interface ITarantoolSpace<T>
    {
        Task<IList<T>> SelectAsync(
            IEnumerable<object> key,
            Iterator iterator = Iterator.Eq,
            uint offset = 0,
            uint limit = int.MaxValue,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> SelectAsync(
            uint indexId,
            IEnumerable<object> key,
            Iterator iterator = Iterator.Eq,
            uint offset = 0,
            uint limit = int.MaxValue,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> SelectAsync(
            string indexName,
            IEnumerable<object> key,
            Iterator iterator = Iterator.Eq,
            uint offset = 0,
            uint limit = int.MaxValue,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> InsertAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> UpdateAsync(
            IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> UpdateAsync(
            uint indexId,
            IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> UpdateAsync(
            string indexName,
            IEnumerable<object> key,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken = default(CancellationToken));

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

        Task<IList<T>> ReplaceAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task UpsertAsync(
            T entity,
            IEnumerable<UpdateOperation> updateOperations,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Index> FindIndexByNameAsync(string indexName,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}