using System.Linq;
using System.Threading.Tasks;
using MsgPack;
using MsgPack.Serialization;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolSpace_SelectShould
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
        public async Task SelectAll()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MessagePackObject>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<uint>(0);

            var result = await testSpacePrimaryIndex.SelectAsync();

            Assert.True(result.Count >= 3);
            Assert.Equal(new[] { "1", "Roxette" }, result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] { "2", "Scorpions", "2015" },
                result[1].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] { "3", "Ace of Base", "1993" },
                result[2].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }

        [Fact]
        public async Task SelectBy1()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MessagePackObject>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<uint>(0);

            var result = await testSpacePrimaryIndex.SelectAsync(1);

            Assert.Equal(1, result.Count);
            Assert.Equal(new[] { "1", "Roxette" }, result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }

        [Fact]
        public async Task SelectBy3()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MessagePackObject>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<uint>(0);

            var result = await testSpacePrimaryIndex.SelectAsync(3);

            Assert.Equal(1, result.Count);
            Assert.Equal(new[] { "3", "Ace of Base", "1993" },
                result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }

        [Fact]
        public async Task SelectEntityBy1()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");
            var testSpace = tarantoolClient.GetSpace<MyTestEntity>("test");
            var testSpacePrimaryIndex = testSpace.GetIndex<uint>(0);

            var result = await testSpacePrimaryIndex.SelectAsync(1);

            Assert.Equal(1, result.Count);
            Assert.Equal(1u, result[0].MyTestEntityId);
            Assert.Equal("Roxette", result[0].SomeStringField);
        }
    }
}