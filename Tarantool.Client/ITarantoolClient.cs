using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    public interface ITarantoolClient
    {
        Task ConnectAsync();

        Task<IList<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage);
    }
}