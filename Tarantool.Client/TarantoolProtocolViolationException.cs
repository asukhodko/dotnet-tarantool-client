using System;

namespace Tarantool.Client
{
    public class TarantoolProtocolViolationException : Exception
    {
        public TarantoolProtocolViolationException(string message):base(message)
        {
        }
    }
}
