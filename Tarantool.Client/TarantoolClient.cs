using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using MsgPack;

using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
using Tarantool.Client.Serialization;

[assembly: InternalsVisibleTo("Tarantool.Client.Tests")]
[assembly: InternalsVisibleTo("Tarantool.Client.FunctionalTests")]

namespace Tarantool.Client
{
    /// <summary>
    ///     The tarantool client.
    /// </summary>
    public class TarantoolClient : ITarantoolClient
    {
        private const int VIndexNameIndexId = 2;

        private const int VIndexSpaceId = 289;

        private const int VSpaceNameIndexId = 2;

        private const int VSpaceSpaceId = 281;

        private readonly IConnectionPool _connectionPool;

        private TarantoolClient(ConnectionOptions connectionOptions)
        {
            _connectionPool = ConnectionPool.GetPool(connectionOptions);
        }

        /// <summary>Initializes a new instance for the <see cref="ITarantoolClient" /> interface with connection options.</summary>
        /// <param name="connectionOptions">The connection options.</param>
        /// <returns>The <see cref="ITarantoolClient" />.</returns>
        public static ITarantoolClient Create(ConnectionOptions connectionOptions)
        {
            return new TarantoolClient(connectionOptions);
        }

        /// <summary>Initializes a new instance for the <see cref="ITarantoolClient" /> interface with connection string.</summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The <see cref="ITarantoolClient" />.</returns>
        public static ITarantoolClient Create(string connectionString)
        {
            return new TarantoolClient(new ConnectionOptions(connectionString));
        }

        /// <summary>Performs a CALL request.</summary>
        /// <param name="callRequest">The call request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with MessagePackObject as result.</returns>
        public async Task<MessagePackObject> CallAsync(CallRequest callRequest, CancellationToken cancellationToken)
        {
            return await RequestAsync(callRequest, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Establish a connection with Tarantool and authenticate.
        ///     Calling this method is redundant as connection establishes automatically during 1st request.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await _connectionPool.ConnectAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Performs an EVAL request.</summary>
        /// <param name="evalRequest">The call request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with MessagePackObject as result.</returns>
        public async Task<MessagePackObject> EvalAsync(EvalRequest evalRequest, CancellationToken cancellationToken)
        {
            return await RequestAsync(evalRequest, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Finds index info by name in space.</summary>
        /// <param name="spaceId">The space id.</param>
        /// <param name="indexName">The index name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with <see cref="Index" /> as result.</returns>
        public async Task<Index> FindIndexByNameAsync(
            uint spaceId,
            string indexName,
            CancellationToken cancellationToken)
        {
            var selectResult = await (await RequestAsyncAsync<Index>(
                                              new SelectRequest
                                              {
                                                  SpaceId = VIndexSpaceId,
                                                  IndexId = VIndexNameIndexId,
                                                  Iterator = Iterator.Eq,
                                                  Key = new List<object> { spaceId, indexName }
                                              },
                                              cancellationToken)
                                          .ConfigureAwait(false)).ConfigureAwait(false);
            return selectResult.FirstOrDefault();
        }

        /// <summary>Find space info by name.</summary>
        /// <param name="spaceName">The space name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with <see cref="Space" /> as result.</returns>
        public async Task<Space> FindSpaceByNameAsync(string spaceName, CancellationToken cancellationToken)
        {
            var selectResult = await (await RequestAsyncAsync<Space>(
                                              new SelectRequest
                                              {
                                                  SpaceId = VSpaceSpaceId,
                                                  IndexId = VSpaceNameIndexId,
                                                  Iterator = Iterator.Eq,
                                                  Key = new List<object> { spaceName }
                                              },
                                              cancellationToken)
                                          .ConfigureAwait(false)).ConfigureAwait(false);
            return selectResult.FirstOrDefault();
        }

        /// <summary>Constructs an <see cref="ITarantoolSpace{T}" /> for working with ORM.</summary>
        /// <param name="spaceId">The space id.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="ITarantoolSpace{T}" />.</returns>
        public ITarantoolSpace<T> GetSpace<T>(uint spaceId)
        {
            return new TarantoolSpace<T>(this, spaceId);
        }

        /// <summary>Constructs an <see cref="ITarantoolSpace{T}" /> for working with ORM.</summary>
        /// <param name="spaceName">The space name.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="ITarantoolSpace{T}" />.</returns>
        public ITarantoolSpace<T> GetSpace<T>(string spaceName)
        {
            return new TarantoolSpace<T>(this, spaceName);
        }

        /// <summary>Performs a general request and fetches response.</summary>
        /// <param name="clientMessage">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with MessagePackObject as result.</returns>
        public async Task<MessagePackObject> RequestAsync(
            ClientMessageBase clientMessage,
            CancellationToken cancellationToken)
        {
            return await (await RequestAsyncAsync(clientMessage, cancellationToken).ConfigureAwait(false))
                       .ConfigureAwait(false);
        }

        /// <summary>Performs a general request.</summary>
        /// <param name="clientMessage">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with Task with MessagePackObject as result.</returns>
        public async Task<Task<MessagePackObject>> RequestAsyncAsync(
            ClientMessageBase clientMessage,
            CancellationToken cancellationToken)
        {
            return await _connectionPool.RequestAsync(clientMessage, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Performs a general request.</summary>
        /// <param name="clientMessage">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The class for object mapping.</typeparam>
        /// <returns>The <see cref="Task" /> with Task with T as result.</returns>
        public async Task<Task<IList<T>>> RequestAsyncAsync<T>(
            ClientMessageBase clientMessage,
            CancellationToken cancellationToken)
        {
            var resultTask = await RequestListAsyncAsync(clientMessage, cancellationToken).ConfigureAwait(false);
            return resultTask.ContinueWith(t => (IList<T>)MapCollection<T>(t.Result).ToList(), cancellationToken);
        }

        /// <summary>Performs a general request and fetches response.</summary>
        /// <param name="clientMessage">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with MessagePackObject as result.</returns>
        public async Task<IList<MessagePackObject>> RequestListAsync(
            ClientMessageBase clientMessage,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await (await RequestListAsyncAsync(clientMessage, cancellationToken).ConfigureAwait(false))
                       .ConfigureAwait(false);
        }

        private IEnumerable<T> MapCollection<T>(IEnumerable<MessagePackObject> source)
        {
            return source.Select(
                x =>
                    {
                        var t = MessagePackObjectMapper.Map<T>(x);
                        return t;
                    });
        }

        /// <summary>Performs a general request.</summary>
        /// <param name="clientMessage">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with Task with IList of MessagePackObject as result.</returns>
        private async Task<Task<IList<MessagePackObject>>> RequestListAsyncAsync(
            ClientMessageBase clientMessage,
            CancellationToken cancellationToken)
        {
            var resultTask = await RequestAsyncAsync(clientMessage, cancellationToken).ConfigureAwait(false);
            return resultTask.ContinueWith(t => t.Result.AsList(), cancellationToken);
        }
    }
}