using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack.Serialization;
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
            await testSpace.DeleteAsync(new List<object> { 598u });
            
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
                await testSpace.DeleteAsync(new List<object> { 598u });
            }
        }
    }
}