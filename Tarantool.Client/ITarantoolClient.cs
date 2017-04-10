using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    public interface ITarantoolClient
    {
        Task ConnectAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<MessagePackObject> RequestAsync(ClientMessageBase clientMessage,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> SelectAsync(SelectRequest selectRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> SelectAsync<T>(SelectRequest selectRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> InsertAsync(InsertRequest insertRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> InsertAsync<T>(InsertRequest<T> insertRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> UpdateAsync(UpdateRequest updateRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> UpdateAsync<T>(UpdateRequest updateRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> DeleteAsync(DeleteRequest deleteRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<MessagePackObject>> ReplaceAsync(ReplaceRequest replaceRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IList<T>> ReplaceAsync<T>(ReplaceRequest<T> replaceRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task UpsertAsync(UpsertRequest upsertRequest, CancellationToken cancellationToken = default(CancellationToken));

        Task UpsertAsync<T>(UpsertRequest<T> upsertRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<MessagePackObject> CallAsync(CallRequest callRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<MessagePackObject> EvalAsync(EvalRequest evalRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Space> FindSpaceByNameAsync(string spaceName,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Index> FindIndexByNameAsync(uint spaceId, string indexName,
            CancellationToken cancellationToken = default(CancellationToken));

        ITarantoolSpace<T> GetSpace<T>(uint spaceId);

        ITarantoolSpace<T> GetSpace<T>(string spaceName);
    }
}