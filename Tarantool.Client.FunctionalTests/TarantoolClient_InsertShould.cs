using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MsgPack.Serialization;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClient_InsertShould
    {
        [Fact]
        public async Task InsertRaw()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;
            await tarantoolClient.RequestAsync(new DeleteRequest
            {
                SpaceId = testSpaceId,
                Key = new List<object> { 99 }
            });

            try
            {
                var result = await tarantoolClient.InsertAsync(new InsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 99, "Some name", 1900 }
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(new[] { "99", "Some name", "1900" },
                    result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeleteRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 99 }
                });
            }
        }

        public class MyTestEntity
        {
            [MessagePackMember(0)]
            public int MyTestEntityId { get; set; }

            [MessagePackMember(1)]
            public string SomeStringField { get; set; }

            [MessagePackMember(2)]
            public int SomeIntField { get; set; }
        }

        [Fact]
        public async Task InsertEntity()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;
            await tarantoolClient.RequestAsync(new DeleteRequest
            {
                SpaceId = testSpaceId,
                Key = new List<object> { 98 }
            });

            try
            {
                var result = await tarantoolClient.InsertAsync(new InsertRequest<MyTestEntity>
                {
                    SpaceId = testSpaceId,
                    Tuple = new MyTestEntity
                    {
                        MyTestEntityId = 98,
                        SomeStringField = "Some name",
                        SomeIntField = 1900
                    }
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(98, result[0].MyTestEntityId);
                Assert.Equal("Some name", result[0].SomeStringField);
                Assert.Equal(1900, result[0].SomeIntField);
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeleteRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 98 }
                });
            }
        }
    }
}