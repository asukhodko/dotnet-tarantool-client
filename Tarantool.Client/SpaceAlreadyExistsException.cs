namespace Tarantool.Client
{
    public class SpaceAlreadyExistsException : TarantoolResponseException
    {
        public SpaceAlreadyExistsException(string message, TarantoolResponseException innerException)
            : base(message, innerException.ResponseCode, innerException)
        {
        }
    }
}