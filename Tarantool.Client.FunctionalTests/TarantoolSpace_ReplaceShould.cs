using System;
using System.Threading.Tasks;
using MsgPack.Serialization;

using Tarantool.Client.Models;
using Xunit;

namespace Tarantool.Client
{
    [Collection("Tarantool database collection")]
    public class TarantoolSpace_ReplaceShould
    {
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
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");
            var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<IndexKey<uint>>(0);

            try
            {
                await testSpace.InsertAsync(new MyTestEntity
                {
                    MyTestEntityId = 576,
                    SomeStringField = "Some name",
                    SomeIntField = 1700
                });

                var result = await testSpace.ReplaceAsync(new MyTestEntity
                {
                    MyTestEntityId = 576,
                    SomeStringField = "Some new name",
                    SomeIntField = 1776
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(576u, result[0].MyTestEntityId);
                Assert.Equal("Some new name", result[0].SomeStringField);
                Assert.Equal(1776, result[0].SomeIntField);
            }
            finally
            {
                await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(576));
            }
        }
    }
}