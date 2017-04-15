namespace Tarantool.Client.Models
{
    /// <summary>The index key of one filed.</summary>
    /// <typeparam name="TK">The type for key mapping</typeparam>
    public class IndexKey<TK> : IndexKey
    {
        /// <summary>Initializes a new instance of the <see cref="IndexKey{TK}"/> class.</summary>
        /// <param name="keyValue">The key value.</param>
        public IndexKey(TK keyValue)
        {
            Key = new object[] { keyValue };
        }
    }
}