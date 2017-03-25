using System;

namespace Tarantool.Client
{
    public class TarantoolException : Exception
    {
        public TarantoolException(string message) : base(message)
        {
        }

        public TarantoolException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}