using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    internal interface IConnectionPool: IDisposable
    {
        Task ConnectAsync();

        Task<IList<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage);
    }
}