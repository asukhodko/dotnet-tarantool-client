namespace Tarantool.Client
{
    internal interface ITarantoolConnection
    {
        void Acquire();

        void Release();

        bool IsAcquired { get; }
    }
}