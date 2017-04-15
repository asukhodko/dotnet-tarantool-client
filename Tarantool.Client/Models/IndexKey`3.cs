namespace Tarantool.Client.Models
{
    /// <summary>The index key of four fileds.</summary>
    /// <typeparam name="TK1">The type for key's part 1 mapping</typeparam>
    /// <typeparam name="TK2">The type for key's part 2 mapping</typeparam>
    /// <typeparam name="TK3">The type for key's part 3 mapping</typeparam>
    public class IndexKey<TK1, TK2, TK3> : IndexKey
    {
        /// <summary>Initializes a new instance of the <see cref="IndexKey{TK1,TK2,TK3}" /> class.</summary>
        /// <param name="keyPart1Value">The key's part 1 value.</param>
        /// <param name="keyPart2Value">The key's part 2 value.</param>
        /// <param name="keyPart3Value">The key's part 3 value.</param>
        public IndexKey(TK1 keyPart1Value, TK2 keyPart2Value, TK3 keyPart3Value)
        {
            Key = new object[] { keyPart1Value, keyPart2Value, keyPart3Value };
        }
    }
}