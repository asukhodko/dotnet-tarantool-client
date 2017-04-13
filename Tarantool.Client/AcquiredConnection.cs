using System.Threading;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client
{
    /// <summary>
    /// The acquired connection. Used for locking connection for send request in multitasking environment.
    /// </summary>
    internal class AcquiredConnection : IAcquiredConnection
    {
        /// <summary>
        /// The Tarantool connection.
        /// </summary>
        private readonly ITarantoolConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcquiredConnection"/> class. 
        /// Marks connection as acquired.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        internal AcquiredConnection(ITarantoolConnection connection)
        {
            _connection = connection;
            _connection.Acquire();
        }

        /// <summary>
        /// Marks connection as non-acquired.
        /// </summary>
        public void Dispose()
        {
            _connection.Release();
        }

        /// <summary>
        /// Translates request to linked acquired connection.
        /// </summary>
        /// <param name="clientMessage">The client message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task"/> for awaiting the result.</returns>
        public Task<Task<MessagePackObject>> RequestAsync(ClientMessageBase clientMessage, CancellationToken cancellationToken)
        {
            return _connection.RequestAsync(clientMessage, cancellationToken);
        }
    }
}