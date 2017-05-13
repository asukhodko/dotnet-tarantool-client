using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    [Collection("Tarantool database collection")]
    public class TarantoolClient_SelectShould
    {
        [Fact]
        public async Task SelectAll()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;

            var result = await tarantoolClient.SelectAsync(new SelectRequest
            {
                SpaceId = testSpaceId,
                Iterator = Iterator.All
            });

            Assert.True(result.Count >= 3);
            Assert.Equal(new[] { "1", "Roxette" }, result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] { "2", "Scorpions", "2015" },
                result[1].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] { "3", "Ace of Base", "1993" },
                result[2].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }


        [Fact]
        public async Task SelectBy1()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;

            var result = await tarantoolClient.SelectAsync(new SelectRequest
            {
                SpaceId = testSpaceId,
                Key = new List<dynamic> { 1 }
            });

            Assert.Equal(1, result.Count);
            Assert.Equal(new[] { "1", "Roxette" }, result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }

        [Fact]
        public async Task SelectBy3()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;

            var result = await tarantoolClient.SelectAsync(new SelectRequest
            {
                SpaceId = testSpaceId,
                Key = new object[] { 3 }
            });

            Assert.Equal(1, result.Count);
            Assert.Equal(new[] { "3", "Ace of Base", "1993" },
                result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }
    }
}