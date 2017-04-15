using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Tarantool.Client.Models;

namespace Tarantool.Client
{
    /// <summary>The TarantoolIndex interface for indexes of one filed.</summary>
    /// <typeparam name="T">The class for object mapping.</typeparam>
    /// <typeparam name="TK">The type for key mapping</typeparam>
    public interface ITarantoolIndex<T, in TK> : ITarantoolIndex<T>
    {
        /// <summary>Select from space by key</summary>
        /// <param name="key">The key value.</param>
        /// <param name="iterator">The iterator.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        Task<IList<T>> SelectAsync(
            TK key,
            Iterator iterator = Iterator.Eq,
            uint offset = 0,
            uint limit = int.MaxValue,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}