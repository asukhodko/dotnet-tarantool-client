using System;
using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    /// <summary>
    /// The AcquiredConnection interface. Used for locking connection during sending request in multitasking environment.
    /// </summary>
    internal interface IAcquiredConnection : IDisposable
    {
        /// <summary>
        /// Translates request to linked acquired connection.
        /// </summary>
        /// <param name="clientMessage">The client message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task"/> for awaiting the result.</returns>
        Task<Task<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage, CancellationToken cancellationToken);
    }
}