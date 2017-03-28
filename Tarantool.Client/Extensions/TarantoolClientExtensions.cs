using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;
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

        public static async Task CreatePrimaryIndexAsync(this TarantoolClient tarantoolClient,
            string spaceName,
            string indexType, // TODO Enum
            string keyType) // TODO Enum
        {
            try
            {
                await tarantoolClient.EvalAsync(
                    $"box.space.{spaceName}:create_index('primary', {{type = '{indexType}',parts = {{1, '{keyType}'}}}})");
            }
            catch (TarantoolResponseException ex)
            {
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