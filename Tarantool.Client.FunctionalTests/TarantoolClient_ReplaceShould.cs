using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClient_ReplaceShould
    {
        [Fact]
        public async Task Replace()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;

            try
            {
                await tarantoolClient.RequestAsync(new InsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 77, "Some name", 1700 }
                });

                var result = await tarantoolClient.ReplaceAsync(new ReplaceRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 77, "Some new name", 1777 }
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "77", "Some new name", "1777" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeleteRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 77 }
                });
            }
        }
    }
}