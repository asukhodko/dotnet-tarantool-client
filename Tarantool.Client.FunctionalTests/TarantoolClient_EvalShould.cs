using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClient_EvalShould
    {
        [Fact]
        public async Task EvaluateScalars()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var result = await tarantoolClient.EvalAsync("return 12345, 23456, 34567");

            Assert.Equal(new[] { 12345, 23456, 34567 }, result.Select(x => x.AsInt32()));
        }

        [Fact]
        public async Task EvaluateScalarsFromArguments()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var result = await tarantoolClient.EvalAsync("return ...", new long[]{912345, 923456, 934567});

            Assert.Equal(new[] { 912345, 923456, 934567 }, result.Select(x => x.AsInt32()));
        }

        /*
          Data setup:
            s = box.space.test
            s:insert({1, 'Roxette'})
            s:insert({2, 'Scorpions', 2015})
            s:insert({3, 'Ace of Base', 1993})
        */
        [Fact]
        public async Task EvaluateSelect()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");
            var result = await tarantoolClient.EvalAsync("box.space.test:select{}");

            // ? Server returns nothing...
            Assert.Equal(3, result.Count);
            Assert.Equal(new[] { "1", "Roxette" }, result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] { "2", "Scorpions", "2015" },
                result[1].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] { "3", "Ace of Base", "1993" },
                result[2].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }
    }
}