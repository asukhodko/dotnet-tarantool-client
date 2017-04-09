using System;
using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    internal interface ITarantoolConnection : IDisposable
    {
        bool IsAcquired { get; }

        Task<Exception> WhenDisconnected { get; }

        bool IsConnected { get; }
        void Acquire();

        void Release();

        Task ConnectAsync(CancellationToken cancellationToken);

        Task<Task<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage,
            CancellationToken cancellationToken);
    }
}