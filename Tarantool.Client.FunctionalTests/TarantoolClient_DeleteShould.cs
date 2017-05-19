using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    [Collection("Tarantool database collection")]
    public class TarantoolClient_DeleteShould
    {
        [Fact]
        public async Task Delete()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;
            await tarantoolClient.RequestAsync(new InsertRequest
            {
                SpaceId = testSpaceId,
                Tuple = new List<object> { 88, "Some name", 1800 }
            });

            var result = await tarantoolClient.RequestListAsync(new DeleteRequest
            {
                SpaceId = testSpaceId,
                Key = new List<object> { 88 }
            });

            Assert.Equal(1, result.Count);
            Assert.Equal(new[] { "88", "Some name", "1800" },
                result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }
    }
}