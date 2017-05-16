using System;
using System.Threading.Tasks;

using MsgPack.Serialization;

using Tarantool.Client.Models;

using Xunit;

namespace Tarantool.Client
{
    [Collection("Tarantool database collection")]
    public class TarantoolIndex_UpdateShould
    {
        [Fact]
        public async Task UpdateByEntityField()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@localhost:3301");
            var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<IndexKey<uint>>(0);
            var now = DateTime.UtcNow;

            try
            {
                await testSpace.InsertAsync(
                    new MyTestEntity { MyTestEntityId = 566, SomeStringField = "Some name", SomeIntField = 1600});

                var result = await testSpacePrimaryIndex.UpdateAsync(
                                 new IndexKey<uint>(566),
                                 new UpdateDefinition<MyTestEntity>()
                                     .AddOpertation(x => x.SomeIntField, UpdateOperationCode.Assign, 1666)
                                     .AddOpertation(x => x.SomeNullableDateTimeField, UpdateOperationCode.Assign, now));

                Assert.Equal(1, result.Count);
                Assert.Equal(566u, result[0].MyTestEntityId);
                Assert.Equal("Some name", result[0].SomeStringField);
                Assert.Equal(1666, result[0].SomeIntField);
                Assert.Equal((now - result[0].SomeNullableDateTimeField.Value).TotalSeconds, 0, 2);
            }
            finally
            {
                await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(566));
            }
        }

        public class MyTestEntity
        {
            [MessagePackMember(0)]
            public uint MyTestEntityId { get; set; }

            [MessagePackMember(3)]
            public DateTime SomeDateTimeField { get; set; }

            [MessagePackMember(2)]
            public int SomeIntField { get; set; }

            [MessagePackMember(5)]
            [MessagePackDateTimeMember(DateTimeConversionMethod = DateTimeMemberConversionMethod.UnixEpoc)]
            public DateTime? SomeNullableDateTimeField { get; set; }

            [MessagePackMember(1)]
            public string SomeStringField { get; set; }
        }
    }
}