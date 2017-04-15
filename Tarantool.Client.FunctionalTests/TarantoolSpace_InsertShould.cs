using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack.Serialization;

using Tarantool.Client.Models;

using Xunit;

namespace Tarantool.Client
{
    public class TarantoolSpace_InsertShould
    {
        public class MyTestEntity
        {
            [MessagePackMember(0)]
            public uint MyTestEntityId { get; set; }

            [MessagePackMember(1)]
            public string SomeStringField { get; set; }

            [MessagePackMember(2)]
            public int SomeIntField { get; set; }
        }

        [Fact]
        public async Task InsertEntity()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<IndexKey<uint>>(0);
            await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(598));

            try
            {
                var result = await testSpace.InsertAsync(new MyTestEntity
                {
                    MyTestEntityId = 598,
                    SomeStringField = "Some name",
                    SomeIntField = 1900
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(598u, result[0].MyTestEntityId);
                Assert.Equal("Some name", result[0].SomeStringField);
                Assert.Equal(1900, result[0].SomeIntField);
            }
            finally
            {
                await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(598));
            }
        }
    }
}