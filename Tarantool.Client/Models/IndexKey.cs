using System.Collections.Generic;

namespace Tarantool.Client.Models
{
    /// <summary>The index key.</summary>
    public abstract class IndexKey
    {
        /// <summary>Gets or sets the key as IEnumerable.</summary>
        public IEnumerable<object> Key { get; protected set; }
    }
}