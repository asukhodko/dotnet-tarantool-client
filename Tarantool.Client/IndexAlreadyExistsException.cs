namespace Tarantool.Client
{
    public class IndexAlreadyExistsException : TarantoolResponseException
    {
        public IndexAlreadyExistsException(string message, TarantoolResponseException innerException)
            : base(message, innerException.ResponseCode, innerException)
        {
        }
    }
}