namespace Tarantool.Client
{
    /// <summary>The index already exists exception.</summary>
    public class IndexAlreadyExistsException : TarantoolResponseException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IndexAlreadyExistsException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public IndexAlreadyExistsException(string message, TarantoolResponseException innerException)
            : base(message, innerException.ResponseCode, innerException)
        {
        }
    }
}