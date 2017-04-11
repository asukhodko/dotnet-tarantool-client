using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tarantool.Client.Models;

namespace Tarantool.Client
{
    public interface ITarantoolIndex<T, in TK>
    {
        Task<IList<T>> SelectAsync(
            TK key,
            Iterator iterator = Iterator.Eq,
            uint offset = 0,
            uint limit = int.MaxValue,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> SelectAsync(
            Iterator iterator = Iterator.All,
            uint offset = 0,
            uint limit = int.MaxValue,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
