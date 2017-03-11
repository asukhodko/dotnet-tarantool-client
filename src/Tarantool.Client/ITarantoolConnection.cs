namespace Tarantool.Client
{
    internal interface ITarantoolConnection
    {
        bool IsAcquired { get; set; }
    }
}