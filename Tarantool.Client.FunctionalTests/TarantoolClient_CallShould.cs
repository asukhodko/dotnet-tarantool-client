using System.Threading.Tasks;
using Tarantool.Client.Models.ClientMessages;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClient_CallShould
    {
        [Fact]
        public async Task CallSomeFunction()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301");

            var result = (await tarantoolClient.CallAsync(new CallRequest
            {
                FunctionName = "some_function"
            })).AsList();

            Assert.True(result.Count == 1);
            Assert.Equal("ok", result[0].AsString());
        }
    }
}