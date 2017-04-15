namespace Tarantool.Client
{
    /// <summary>Represents error when Tarantool space index is already exists.</summary>
    public class IndexAlreadyExistsException : TarantoolResponseException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IndexAlreadyExistsException" /> class with a specified error
        ///     message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public IndexAlreadyExistsException(string message, TarantoolResponseException innerException)
            : base(message, innerException.ResponseCode, innerException)
        {
        }
    }
}