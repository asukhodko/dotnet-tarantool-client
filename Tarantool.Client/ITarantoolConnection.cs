using System;
using System.Threading;
using System.Threading.Tasks;

using MsgPack;

using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    /// <summary>
    ///     The TarantoolConnection interface.
    /// </summary>
    internal interface ITarantoolConnection : IDisposable
    {
        /// <summary>Gets a value indicating whether connection is now in use for sending request.</summary>
        bool IsAcquired { get; }

        /// <summary>Gets a value indicating whether connection is still active.</summary>
        bool IsConnected { get; }

        /// <summary>Mark connection as in use for sending request.</summary>
        void Acquire();

        /// <summary>Connect to Tarantool and authenticate.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" />.</returns>
        Task ConnectAsync(CancellationToken cancellationToken);

        /// <summary>Schedules the continuation action when disconnection happens.</summary>
        /// <param name="continuation">The action to invoke on disconnection.</param>
        void OnDisconnected(Action continuation);

        /// <summary>Mark connection as not in use for sending request.</summary>
        void Release();

        /// <summary>Send request to Tarantool server.</summary>
        /// <param name="clientMessage">The client message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task" /> for awaiting the result.</returns>
        Task<Task<MessagePackObject>> RequestAsync(
            ClientMessageBase clientMessage,
            CancellationToken cancellationToken);
    }
}