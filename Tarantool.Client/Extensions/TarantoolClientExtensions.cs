using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MsgPack;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;

namespace Tarantool.Client.Extensions
{
    public static class TarantoolClientExtensions
    {
        /// <exception cref="SpaceAlreadyExistsException"></exception>
        /// <exception cref="TarantoolResponseException"></exception>
        public static async Task CreateSpaceAsync(this TarantoolClient tarantoolClient, string spaceName)
        {
            try
            {
                await tarantoolClient.EvalAsync($"box.schema.create_space('{spaceName}')");
            }
            catch (TarantoolResponseException ex)
            {
                if (ex.Message.StartsWith("Space") && ex.Message.EndsWith("already exists"))
                    throw new SpaceAlreadyExistsException(ex.Message, ex);
                throw;
            }
        }

        public static async Task DropSpaceAsync(this TarantoolClient tarantoolClient, string spaceName)
        {
            await tarantoolClient.EvalAsync($"box.space.{spaceName}:drop()");
        }

        /// <exception cref="ArgumentException">parts is null or empty.</exception>
        public static async Task CreateIndexAsync(this TarantoolClient tarantoolClient,
            string spaceName,
            string indexName,
            IndexType indexType,
            params IndexPart[] parts)
        {
            try
            {
                if (parts == null || !parts.Any())
                    throw new ArgumentException(nameof(parts));
                var partsString = string.Join(",", parts.Select(x => $"{x.FieldNumber + 1}, '{x.Type.ToString()}'"));
                await tarantoolClient.EvalAsync(
                    $"box.space.{spaceName}:create_index('{indexName}', {{type = '{indexType}', parts = {{{partsString}}}}})");
            }
            catch (TarantoolResponseException ex)
            {
                if (ex.Message.StartsWith("Index") && ex.Message.EndsWith("already exists"))
                    throw new IndexAlreadyExistsException(ex.Message, ex);
                throw;
            }
        }

        public static Task<IList<MessagePackObject>> EvalAsync(this TarantoolClient tarantoolClient, string expression,
            params object[] args)
        {
            return tarantoolClient.RequestAsync(new EvalRequest
            {
                Expression = expression,
                Args = args
            });
        }
    }
}