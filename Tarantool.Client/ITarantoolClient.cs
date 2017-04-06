using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    public interface ITarantoolClient
    {
        Task ConnectAsync();

        Task<MessagePackObject> RequestAsync(ClientMessageBase clientMessage);

        Task<IList<MessagePackObject>> SelectAsync(SelectRequest selectRequest);

        Task<IList<T>> SelectAsync<T>(SelectRequest selectRequest) where T : new();

        Task<IList<MessagePackObject>> InsertAsync(InsertRequest insertRequest);

        Task<IList<MessagePackObject>> UpdateAsync(UpdateRequest updateRequest);

        Task<IList<MessagePackObject>> DeleteAsync(DeleteRequest deleteRequest);

        Task<IList<MessagePackObject>> ReplaceAsync(ReplaceRequest replaceRequest);

        Task UpsertAsync(UpsertRequest upsertRequest);

        Task<MessagePackObject> CallAsync(CallRequest callRequest);

        Task<MessagePackObject> EvalAsync(EvalRequest evalRequest);

        Task<Space> FindSpaceByNameAsync(string spaceName);

        Task<Index> FindIndexByNameAsync(uint spaceId, string indexName);

    }
}