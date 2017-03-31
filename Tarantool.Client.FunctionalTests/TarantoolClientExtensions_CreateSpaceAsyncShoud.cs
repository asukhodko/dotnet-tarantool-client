using System.Threading.Tasks;
using Tarantool.Client.Extensions;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClientExtensions_CreateSpaceAsyncShoud
    {
        [Fact]
        public async Task CreateSpace()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");

            try
            {
                await tarantoolClient.CreateSpaceAsync("test2");

                var result = await tarantoolClient.FindSpaceByNameAsync("test2");
                Assert.NotNull(result);
                Assert.True(result.Count >= 2);
                Assert.Equal("test2", result[2].AsString());
            }
            finally
            {
                await tarantoolClient.DropSpaceAsync("test2");
            }
        }

        [Fact]
        public async Task HandleSpaceExists()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");

            try
            {
                await Assert.ThrowsAsync<SpaceAlreadyExistsException>(async () =>
                {
                    await tarantoolClient.CreateSpaceAsync("test3");

                    await tarantoolClient.CreateSpaceAsync("test3");
                });
            }
            finally
            {
                await tarantoolClient.DropSpaceAsync("test3");
            }

        }

    }
}
