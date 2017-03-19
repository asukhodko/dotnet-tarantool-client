using System;
using System.Threading.Tasks;

namespace Tarantool.Client
{
    internal interface ITarantoolConnection: IDisposable
    {
        void Acquire();

        void Release();

        bool IsAcquired { get; }

        Task ConnectAsync();

        Task<Exception> WhenDisconnected { get; }

        bool IsConnected { get; }
    }
}