using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClient_SelectShould
    {
        /*
          Data setup:
            s = box.space.test
            s:insert({1, 'Roxette'})
            s:insert({2, 'Scorpions', 2015})
            s:insert({3, 'Ace of Base', 1993})
        */

        [Fact]
        public async Task ReturnData()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301");

            var result = await tarantoolClient.SelectAsync(514, 0);

            Assert.Equal(3, result.Count);
            Assert.Equal(new[] {"1", "Roxette"}, result[0].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] {"2", "Scorpions", "2015"},
                result[1].AsList().Select(x => x.ToObject().ToString()).ToArray());
            Assert.Equal(new[] {"3", "Ace of Base", "1993"},
                result[2].AsList().Select(x => x.ToObject().ToString()).ToArray());
        }
    }
}