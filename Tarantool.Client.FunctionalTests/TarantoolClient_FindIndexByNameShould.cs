using System.Threading.Tasks;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClient_FindIndexByNameShould
    {
        [Fact]
        public async Task SelectIndexId()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");

            var result = await tarantoolClient.FindIndexByNameAsync(281, "owner");

            Assert.NotNull(result);
            Assert.Equal(281, result[0].AsInt32());
            Assert.Equal(1, result[1].AsInt32());
            Assert.Equal("owner", result[2].AsString());
        }
    }
}