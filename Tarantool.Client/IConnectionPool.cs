using System;
using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    internal interface IConnectionPool: IDisposable
    {
        Task ConnectAsync(CancellationToken cancellationToken);

        Task<MessagePackObject> RequestAsync(ClientMessageBase clientMessage, CancellationToken cancellationToken);
    }
}