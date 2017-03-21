using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    internal interface IAcquiredConnection : IDisposable
    {
        Task<Task<IList<MessagePackObject>>> RequestAsync(ClientMessageBase clientMessage);
    }
}