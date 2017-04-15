using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MsgPack;

using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    /// <summary>
    ///     The TarantoolClient interface.
    /// </summary>
    public interface ITarantoolClient
    {
        /// <summary>Performs a CALL request.</summary>
        /// <param name="callRequest">The call request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with MessagePackObject as result.</returns>
        Task<MessagePackObject> CallAsync(
            CallRequest callRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Establish a connection with Tarantool and authenticate.
        ///     Calling this method is redundant as connection establishes automatically during 1st request.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        Task ConnectAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a DELETE request.</summary>
        /// <param name="deleteRequest">The delete request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///     The <see cref="Task" /> with list of deleted rows.
        /// </returns>
        Task<IList<MessagePackObject>> DeleteAsync(
            DeleteRequest deleteRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs an EVAL request.</summary>
        /// <param name="evalRequest">The call request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with MessagePackObject as result.</returns>
        Task<MessagePackObject> EvalAsync(
            EvalRequest evalRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Finds index info by name in space.</summary>
        /// <param name="spaceId">The space id.</param>
        /// <param name="indexName">The index name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with <see cref="Index" /> as result.</returns>
        Task<Index> FindIndexByNameAsync(
            uint spaceId,
            string indexName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Find space info by name.</summary>
        /// <param name="spaceName">The space name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with <see cref="Space" /> as result.</returns>
        Task<Space> FindSpaceByNameAsync(
            string spaceName,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Constructs an <see cref="ITarantoolSpace{T}" /> for working with ORM.</summary>
        /// <param name="spaceId">The space id.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="ITarantoolSpace{T}" />.</returns>
        ITarantoolSpace<T> GetSpace<T>(uint spaceId);

        /// <summary>Constructs an <see cref="ITarantoolSpace{T}" /> for working with ORM.</summary>
        /// <param name="spaceName">The space name.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="ITarantoolSpace{T}" />.</returns>
        ITarantoolSpace<T> GetSpace<T>(string spaceName);

        /// <summary>Performs an INSERT request.</summary>
        /// <param name="insertRequest">The insert request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with inserted data as result.</returns>
        Task<IList<MessagePackObject>> InsertAsync(
            InsertRequest insertRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs an INSERT request.</summary>
        /// <param name="insertRequest">The insert request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="Task" /> with inserted data as result.</returns>
        Task<IList<T>> InsertAsync<T>(
            InsertRequest<T> insertRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a REPLACE request.</summary>
        /// <param name="replaceRequest">The replace request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with replaced data as result.</returns>
        Task<IList<MessagePackObject>> ReplaceAsync(
            ReplaceRequest replaceRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a REPLACE request.</summary>
        /// <param name="replaceRequest">The replace request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="Task" /> with replaced data as result.</returns>
        Task<IList<T>> ReplaceAsync<T>(
            ReplaceRequest<T> replaceRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a general request.</summary>
        /// <param name="clientMessage">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with MessagePackObject as result.</returns>
        Task<MessagePackObject> RequestAsync(
            ClientMessageBase clientMessage,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a SELECT request.</summary>
        /// <param name="selectRequest">The select request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with replaced data as result.</returns>
        Task<IList<MessagePackObject>> SelectAsync(
            SelectRequest selectRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a SELECT request.</summary>
        /// <param name="selectRequest">The select request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="Task" /> with selected data as result.</returns>
        Task<IList<T>> SelectAsync<T>(
            SelectRequest selectRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a UPDATE request.</summary>
        /// <param name="updateRequest">The update request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with updated data as result.</returns>
        Task<IList<MessagePackObject>> UpdateAsync(
            UpdateRequest updateRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a UPDATE request.</summary>
        /// <param name="updateRequest">The update request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="Task" /> with updated data as result.</returns>
        Task<IList<T>> UpdateAsync<T>(
            UpdateRequest updateRequest,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a UPSERT (update-or-insert) request.</summary>
        /// <param name="upsertRequest">The upsert request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with no data as result.</returns>
        Task UpsertAsync(UpsertRequest upsertRequest, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Performs a UPSERT (update-or-insert) request.</summary>
        /// <param name="upsertRequest">The upsert request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="Task" /> with no data as result.</returns>
        Task UpsertAsync<T>(
            UpsertRequest<T> upsertRequest,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}