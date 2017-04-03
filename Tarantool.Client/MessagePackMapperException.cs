using System;

namespace Tarantool.Client
{
    public class MessagePackMapperException : TarantoolException
    {
        public MessagePackMapperException(string message) : base(message)
        {
        }

        public MessagePackMapperException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
