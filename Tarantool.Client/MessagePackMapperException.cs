using System;

namespace Tarantool.Client
{
    /// <summary>Represents errors that occur during mapping MessagePack objects.</summary>
    public class MessagePackMapperException : TarantoolException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MessagePackMapperException" /> class with a specified error
        ///     message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public MessagePackMapperException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessagePackMapperException" /> class with a specified error
        ///     message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MessagePackMapperException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}