using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MsgPack.Serialization;
using Tarantool.Client.Models;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolIndex_UpdateShould
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
        public async Task Update()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<IndexKey<uint>>(0);

            try
            {
                await testSpace.InsertAsync(new MyTestEntity
                {
                    MyTestEntityId = 566,
                    SomeStringField = "Some name",
                    SomeIntField = 1600
                });

                var result = await testSpacePrimaryIndex.UpdateAsync(
                    new IndexKey<uint>(566),
                    new[]
                    {
                        new UpdateOperation<int>
                        {
                            Operation = UpdateOperationCode.Assign,
                            FieldNo = 2,
                            Argument = 1666
                        }
                    });

                Assert.Equal(1, result.Count);
                Assert.Equal(566u, result[0].MyTestEntityId);
                Assert.Equal("Some name", result[0].SomeStringField);
                Assert.Equal(1666, result[0].SomeIntField);
            }
            finally
            {
                await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(566));
            }
        }
    }
}