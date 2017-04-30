using System;
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

            [MessagePackMember(3)]
            public DateTime SomeDateTimeField { get; set; }

            [MessagePackMember(4)]
            [MessagePackDateTimeMember(DateTimeConversionMethod = DateTimeMemberConversionMethod.Native)]
            public DateTime SomeDateTimeFieldN { get; set; }

            [MessagePackMember(5)]
            [MessagePackDateTimeMember(DateTimeConversionMethod = DateTimeMemberConversionMethod.UnixEpoc)]
            public DateTime SomeDateTimeFieldU { get; set; }

            [MessagePackMember(6)]
            public DateTime? SomeNullableDateTimeField { get; set; }

            [MessagePackMember(7)]
            public DateTime? SomeNullableDateTimeField2 { get; set; }
        }

        [Fact]
        public async Task InsertEntity()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<IndexKey<uint>>(0);
            await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(598));
            var now = DateTime.UtcNow;

            try
            {
                var result = await testSpace.InsertAsync(new MyTestEntity
                {
                    MyTestEntityId = 598,
                    SomeStringField = "Some name",
                    SomeIntField = 1900,
                    SomeDateTimeField = now,
                    SomeDateTimeFieldN = now,
                    SomeDateTimeFieldU = now,
                    SomeNullableDateTimeField = now,
                    SomeNullableDateTimeField2 = null
                });

                Assert.Equal(1, result.Count);
                Assert.Equal(598u, result[0].MyTestEntityId);
                Assert.Equal("Some name", result[0].SomeStringField);
                Assert.Equal(1900, result[0].SomeIntField);
                Assert.Equal(now, result[0].SomeDateTimeField);
                Assert.Equal(now, result[0].SomeDateTimeFieldN);
                Assert.Equal((now - result[0].SomeDateTimeFieldU).TotalSeconds, 0, 2);
            }
            finally
            {
                await testSpacePrimaryIndex.DeleteAsync(new IndexKey<uint>(598));
            }
        }
    }
}