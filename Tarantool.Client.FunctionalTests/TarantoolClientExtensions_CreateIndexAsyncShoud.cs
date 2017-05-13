using System.Threading.Tasks;
using Tarantool.Client.Extensions;
using Tarantool.Client.Models;
using Xunit;

namespace Tarantool.Client
{
    [Collection("Tarantool database collection")]
    public class TarantoolClientExtensions_CreateIndexAsyncShoud
    {
        [Fact]
        public async Task CreateIndex()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");

            try
            {
                await tarantoolClient.CreateSpaceAsync("testforindex1");

                await tarantoolClient.CreateIndexAsync("testforindex1", "primary", IndexType.Hash,
                    new IndexPart(0, IndexedFieldType.Unsigned));

                var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testforindex1")).SpaceId;
                var result = await tarantoolClient.FindIndexByNameAsync(spaceId, "primary");
                Assert.NotNull(result);
                Assert.NotNull(result);
                Assert.Equal(spaceId, result.SpaceId);
                Assert.Equal("primary", result.Name);
                Assert.Equal(IndexType.Hash, result.IndexType);
            }
            finally
            {
                await tarantoolClient.DropSpaceAsync("testforindex1");
            }
        }

        [Fact]
        public async Task DropIndex()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");

            try
            {
                await tarantoolClient.CreateSpaceAsync("testforindex3");
                await tarantoolClient.CreateIndexAsync("testforindex3", "primary", IndexType.Hash,
                    new IndexPart(0, IndexedFieldType.Unsigned));

                await tarantoolClient.DropIndexAsync("testforindex3", "primary");

                var spaceId = (await tarantoolClient.FindSpaceByNameAsync("testforindex3")).SpaceId;
                var result = await tarantoolClient.FindIndexByNameAsync(spaceId, "primary");
                Assert.Null(result);
            }
            finally
            {
                await tarantoolClient.DropSpaceAsync("testforindex3");
            }
        }

        [Fact]
        public async Task HandleIndexExists()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");

            try
            {
                await Assert.ThrowsAsync<IndexAlreadyExistsException>(async () =>
                {
                    await tarantoolClient.CreateSpaceAsync("testforindex2");
                    await tarantoolClient.CreateIndexAsync("testforindex2", "primary", IndexType.Hash,
                        new IndexPart(0, IndexedFieldType.Unsigned));

                    await tarantoolClient.CreateIndexAsync("testforindex2", "primary", IndexType.Hash,
                        new IndexPart(0, IndexedFieldType.Unsigned));
                });
            }
            finally
            {
                await tarantoolClient.DropSpaceAsync("testforindex2");
            }
        }
    }
}