using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MsgPack.Serialization;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClient_ReplaceShould
    {
        [Fact]
        public async Task ReplaceRow()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
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

        public class MyTestEntity
        {
            [MessagePackMember(0)]
            public uint MyTestEntityId { get; set; }

            [MessagePackMember(1)]
            public string SomeStringField { get; set; }

            [MessagePackMember(2)]
            public int SomeIntField { get; set; }

            [MessagePackMember(3)]
            public DateTime SomeDateTimeField { get; set; }
        }

        [Fact]
        public async Task ReplaceEntity()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpaceId = (await tarantoolClient.FindSpaceByNameAsync("test")).SpaceId;

            try
            {
                await tarantoolClient.RequestAsync(new InsertRequest
                {
                    SpaceId = testSpaceId,
                    Tuple = new List<object> { 76, "Some name", 1700 }
                });

                var result = await tarantoolClient.ReplaceAsync(new ReplaceRequest<MyTestEntity>
                {
                    SpaceId = testSpaceId,
                    Tuple = new MyTestEntity
                    {
                        MyTestEntityId = 76,
                        SomeStringField = "Some new name",
                        SomeIntField = 1776
                    }
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(76u, result[0].MyTestEntityId);
                Assert.Equal("Some new name", result[0].SomeStringField);
                Assert.Equal(1776, result[0].SomeIntField);
            }
            finally
            {
                await tarantoolClient.RequestAsync(new DeleteRequest
                {
                    SpaceId = testSpaceId,
                    Key = new List<object> { 76 }
                });
            }
        }
    }
}