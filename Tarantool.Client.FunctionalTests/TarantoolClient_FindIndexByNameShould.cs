using System.Threading.Tasks;
using Xunit;

namespace Tarantool.Client
{
    [Collection("Tarantool database collection")]
    public class TarantoolClient_FindIndexByNameShould
    {
        [Fact]
        public async Task SelectIndexId()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");

            var result = await tarantoolClient.FindIndexByNameAsync(281, "owner");

            Assert.NotNull(result);
            Assert.Equal(281u, result.SpaceId);
            Assert.Equal(1u, result.IndexId);
            Assert.Equal("owner", result.Name);
        }
    }
}