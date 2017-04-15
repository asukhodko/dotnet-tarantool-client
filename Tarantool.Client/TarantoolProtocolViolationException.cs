namespace Tarantool.Client
{
    /// <summary>Represents errors with parsing messages from Tarantool server.</summary>
    public class TarantoolProtocolViolationException : TarantoolException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TarantoolProtocolViolationException" /> class with a specified
        ///     error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public TarantoolProtocolViolationException(string message)
            : base(message)
        {
        }
    }
}