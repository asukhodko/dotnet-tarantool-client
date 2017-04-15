namespace Tarantool.Client
{
    /// <summary>Represents error when Tarantool space is already exists.</summary>
    public class SpaceAlreadyExistsException : TarantoolResponseException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpaceAlreadyExistsException" /> class with a specified error
        ///     message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public SpaceAlreadyExistsException(string message, TarantoolResponseException innerException)
            : base(message, innerException.ResponseCode, innerException)
        {
        }
    }
}