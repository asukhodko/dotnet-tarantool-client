using System;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    internal interface IConnectionPool: IDisposable
    {
        Task ConnectAsync();

        Task<MessagePackObject> RequestAsync(ClientMessageBase clientMessage);
    }
}