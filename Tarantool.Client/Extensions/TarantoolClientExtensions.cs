using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MsgPack;

using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client.Extensions
{
    public static class TarantoolClientExtensions
    {
        /// <exception cref="ArgumentException">parts is null or empty.</exception>
        /// <exception cref="IndexAlreadyExistsException"></exception>
        /// <exception cref="TarantoolResponseException"></exception>
        public static async Task CreateIndexAsync(
            this ITarantoolClient tarantoolClient,
            string spaceName,
            string indexName,
            IndexType indexType,
            params IndexPart[] parts)
        {
            try
            {
                if (parts == null || !parts.Any()) throw new ArgumentException(nameof(parts));
                var partsString = string.Join(",", parts.Select(x => $"{x.FieldNumber + 1}, '{x.Type.ToString()}'"));
                await tarantoolClient.EvalAsync(
                        $"box.space.{spaceName}:create_index('{indexName}', {{type = '{indexType}', parts = {{{partsString}}}}})")
                    .ConfigureAwait(false);
            }
            catch (TarantoolResponseException ex)
            {
                if (ex.Message.StartsWith("Index") && ex.Message.EndsWith("already exists"))
                    throw new IndexAlreadyExistsException(ex.Message, ex);
                throw;
            }
        }

        /// <exception cref="ArgumentException">parts is null or empty.</exception>
        /// <exception cref="IndexAlreadyExistsException"></exception>
        /// <exception cref="TarantoolResponseException"></exception>
        public static async Task CreateIndexAsync(
            this ITarantoolClient tarantoolClient,
            string spaceName,
            string indexName,
            IndexType indexType,
            CancellationToken cancellationToken,
            params IndexPart[] parts)
        {
            try
            {
                if (parts == null || !parts.Any()) throw new ArgumentException(nameof(parts));
                var partsString = string.Join(",", parts.Select(x => $"{x.FieldNumber + 1}, '{x.Type.ToString()}'"));
                await tarantoolClient.EvalAsync(
                        $"box.space.{spaceName}:create_index('{indexName}', {{type = '{indexType}', parts = {{{partsString}}}}})",
                        cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TarantoolResponseException ex)
            {
                if (ex.Message.StartsWith("Index") && ex.Message.EndsWith("already exists"))
                    throw new IndexAlreadyExistsException(ex.Message, ex);
                throw;
            }
        }

        /// <exception cref="TarantoolResponseException"></exception>
        public static async Task CreateIndexIfNotExistsAsync(
            this ITarantoolClient tarantoolClient,
            string spaceName,
            string indexName,
            IndexType indexType,
            params IndexPart[] parts)
        {
            try
            {
                await tarantoolClient.CreateIndexAsync(spaceName, indexName, indexType, parts).ConfigureAwait(false);
            }
            catch (IndexAlreadyExistsException)
            {
            }
        }

        /// <exception cref="TarantoolResponseException"></exception>
        public static async Task CreateIndexIfNotExistsAsync(
            this ITarantoolClient tarantoolClient,
            string spaceName,
            string indexName,
            IndexType indexType,
            CancellationToken cancellationToken,
            params IndexPart[] parts)
        {
            try
            {
                await tarantoolClient.CreateIndexAsync(spaceName, indexName, indexType, cancellationToken, parts)
                    .ConfigureAwait(false);
            }
            catch (IndexAlreadyExistsException)
            {
            }
        }

        /// <exception cref="SpaceAlreadyExistsException"></exception>
        /// <exception cref="TarantoolResponseException"></exception>
        public static async Task CreateSpaceAsync(
            this ITarantoolClient tarantoolClient,
            string spaceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await tarantoolClient.EvalAsync($"box.schema.create_space('{spaceName}')", cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TarantoolResponseException ex)
            {
                if (ex.Message.StartsWith("Space") && ex.Message.EndsWith("already exists"))
                    throw new SpaceAlreadyExistsException(ex.Message, ex);
                throw;
            }
        }

        /// <exception cref="TarantoolResponseException"></exception>
        public static async Task CreateSpaceIfNotExistsAsync(
            this ITarantoolClient tarantoolClient,
            string spaceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await tarantoolClient.CreateSpaceAsync(spaceName, cancellationToken).ConfigureAwait(false);
            }
            catch (SpaceAlreadyExistsException)
            {
            }
        }

        public static async Task DropIndexAsync(
            this ITarantoolClient tarantoolClient,
            string spaceName,
            string indexName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await tarantoolClient.EvalAsync($"box.space.{spaceName}.index.{indexName}:drop()", cancellationToken)
                .ConfigureAwait(false);
        }

        public static async Task DropSpaceAsync(
            this ITarantoolClient tarantoolClient,
            string spaceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await tarantoolClient.EvalAsync($"box.space.{spaceName}:drop()", cancellationToken).ConfigureAwait(false);
        }

        public static Task<MessagePackObject> EvalAsync(
            this ITarantoolClient tarantoolClient,
            string expression,
            params object[] args)
        {
            return tarantoolClient.RequestAsync(new EvalRequest { Expression = expression, Args = args });
        }

        public static Task<MessagePackObject> EvalAsync(
            this ITarantoolClient tarantoolClient,
            string expression,
            CancellationToken cancellationToken,
            params object[] args)
        {
            return tarantoolClient.RequestAsync(
                new EvalRequest { Expression = expression, Args = args },
                cancellationToken);
        }
    }
}