using System;

namespace Tarantool.Client
{
    /// <summary>Represents errors received from Tarantool server.</summary>
    public class TarantoolResponseException : TarantoolException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TarantoolResponseException" /> class with a specified error
        ///     message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="responseCode">The response code received from Tarantool server.</param>
        public TarantoolResponseException(string message, int responseCode)
            : base(message)
        {
            ResponseCode = responseCode;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TarantoolResponseException" /> class with a specified error
        ///     message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="responseCode">The response code received from Tarantool server.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        protected TarantoolResponseException(string message, int responseCode, Exception innerException)
            : base(message, innerException)
        {
            ResponseCode = responseCode;
        }

        /// <summary>Gets the response code received from Tarantool server.</summary>
        public int ResponseCode { get; }
    }
}