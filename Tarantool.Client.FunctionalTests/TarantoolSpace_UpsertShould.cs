using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack.Serialization;
using Tarantool.Client.Models;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolSpace_UpsertShould
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
        public async Task UpsertAsInsert()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<IndexKey<uint>>(0);

            try
            {
                await testSpace.UpsertAsync(
                    new MyTestEntity { MyTestEntityId = 555, SomeStringField = "Some name", SomeIntField = 1550 },
                    new UpdateDefinition<MyTestEntity>().AddOpertation(
                        new UpdateOperation<MyTestEntity, int>(x => x.SomeIntField)
                            {
                                Operation =
                                    UpdateOperationCode.Assign,
                                Argument = 1555
                            }));

                var result = await testSpacePrimaryIndex.SelectAsync(new IndexKey<uint>(555));
                Assert.Equal(1, result.Count);
                Assert.Equal(555u, result[0].MyTestEntityId);
                Assert.Equal("Some name", result[0].SomeStringField);
                Assert.Equal(1550, result[0].SomeIntField);
            }
            finally
            {
                await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(555));
            }
        }

        [Fact]
        public async Task UpsertAsUpdate()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<IndexKey<uint>>(0);

            try
            {
                await testSpace.InsertAsync(new MyTestEntity
                {
                    MyTestEntityId = 544,
                    SomeStringField = "Some name",
                    SomeIntField = 1400
                });

                await testSpace.UpsertAsync(
                    new MyTestEntity
                    {
                        MyTestEntityId = 544,
                        SomeStringField = "Some name",
                        SomeIntField = 1440
                    },
                    new UpdateDefinition<MyTestEntity>().AddOpertation(
                        new UpdateOperation<MyTestEntity, int>(x => x.SomeIntField)
                        {
                            Operation = UpdateOperationCode.Assign,
                            Argument = 1444
                        }
                    ));

                var result = await testSpacePrimaryIndex.SelectAsync(new IndexKey<uint>(544));
                Assert.Equal(1, result.Count);
                Assert.Equal(544u, result[0].MyTestEntityId);
                Assert.Equal("Some name", result[0].SomeStringField);
                Assert.Equal(1444, result[0].SomeIntField);
            }
            finally
            {
                await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(544));
            }
        }
    }
}