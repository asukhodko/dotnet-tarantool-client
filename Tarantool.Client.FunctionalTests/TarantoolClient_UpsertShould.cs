using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClient_UpsertShould
    {
        [Fact]
        public async Task UpsertAsInsert()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;

            try
            {
                await tarantoolClient.UpsertAsync(new UpsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 55, "Some name", 1550 },
                    UpdateUperations = new[]
                    {
                        new UpdateOperation<int>
                        {
                            Operation = UpdateOperationCode.Assign,
                            FieldNo = 2,
                            Argument = 1555
                        }
                    }
                });

                var result = await tarantoolClient.SelectAsync(new SelectRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 55 }
                });
                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "55", "Some name", "1550" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeleteRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 55 }
                });
            }
        }

        [Fact]
        public async Task UpsertAsUpdate()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;

            try
            {
                await tarantoolClient.RequestAsync(new InsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 44, "Some name", 1400 }
                });

                await tarantoolClient.UpsertAsync(new UpsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 44, "Some name", 1440 },
                    UpdateUperations = new[]
                    {
                        new UpdateOperation<int>
                        {
                            Operation = UpdateOperationCode.Assign,
                            FieldNo = 2,
                            Argument = 1444
                        }
                    }
                });

                var result = await tarantoolClient.SelectAsync(new SelectRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 44 }
                });
                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "44", "Some name", "1444" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeleteRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 44 }
                });
            }
        }
    }
}