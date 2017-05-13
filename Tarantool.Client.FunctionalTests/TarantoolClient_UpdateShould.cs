using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    [Collection("Tarantool database collection")]
    public class TarantoolClient_UpdateShould
    {
        [Fact]
        public async Task Update()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;

            try
            {
                await tarantoolClient.RequestAsync(new InsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 66, "Some name", 1600 }
                });

                var result = await tarantoolClient.UpdateAsync(new UpdateRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 66 },
                    UpdateOperations = new[]
                    {
                        new UpdateOperation<int>
                        {
                            Operation = UpdateOperationCode.Assign,
                            FieldNo = 2,
                            Argument = 1666
                        }
                    }
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "66", "Some name", "1666" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeleteRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 66 }
                });
            }
        }
    }
}