using System.Threading.Tasks;
using Xunit;

namespace Tarantool.Client
{
    [Collection("Tarantool database collection")]
    public class TarantoolClient_FindSpaceByNameShould
    {
        [Fact]
        public async Task SelectSpaceId()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");

            var result = await tarantoolClient.FindSpaceByNameAsync("_vspace");

            Assert.NotNull(result);
            Assert.Equal(281u, result.SpaceId);
            Assert.Equal("_vspace", result.Name);
        }
    }
}