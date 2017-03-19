using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;

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

        Task<IList<MessagePackObject>> EvalAsync(string expression, long[] args);
    }
}