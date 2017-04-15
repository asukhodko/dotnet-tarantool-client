using System;
using System.Threading;
using System.Threading.Tasks;

using MsgPack;

using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    /// <summary>The ConnectionPool interface.</summary>
    internal interface IConnectionPool : IDisposable
    {
        /// <summary>
        ///     Establish a connection with Tarantool and authenticate.
        ///     Calling this method is redundant as connection establishes automatically during 1st request.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        Task ConnectAsync(CancellationToken cancellationToken);

        /// <summary>Send request to Tarantool server and get response.</summary>
        /// <param name="clientMessage">The client message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> with response.</returns>
        Task<MessagePackObject> RequestAsync(ClientMessageBase clientMessage, CancellationToken cancellationToken);
    }
}