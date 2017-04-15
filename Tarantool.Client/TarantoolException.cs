using System;

namespace Tarantool.Client
{
    /// <summary>Represents errors that occur during Tarantool client execution.</summary>
    public class TarantoolException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="TarantoolException" /> class with a specified error message.</summary>
        /// <param name="message">The message that describes the error. </param>
        public TarantoolException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TarantoolException" /> class with a specified error message and a
        ///     reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public TarantoolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}