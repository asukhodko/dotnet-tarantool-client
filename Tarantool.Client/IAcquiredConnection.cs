using System;
using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    internal interface IAcquiredConnection : IDisposable
    {
        Task<Task<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage, CancellationToken cancellationToken);
    }
}