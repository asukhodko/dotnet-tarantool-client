using System;

namespace Tarantool.Client
{
    public class TarantoolResponseException : TarantoolException
    {
        public TarantoolResponseException(string message, int responseCode) : base(message)
        {
            ResponseCode = responseCode;
        }

        protected TarantoolResponseException(string message, int responseCode, Exception innerException)
            : base(message, innerException)
        {
            ResponseCode = responseCode;
        }

        public int ResponseCode { get; set; }
    }
}