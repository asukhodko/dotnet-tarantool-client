using System.Threading.Tasks;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClientShould
    {
        /*
          Tarantool setup:
            box.schema.user.create('mytestuser', { password = 'mytestpass' })
            box.schema.user.grant('mytestuser', 'read,write', 'space', '_schema')
            box.schema.user.grant('mytestuser', 'read,write', 'space', '_space')
            box.schema.user.grant('mytestuser', 'read,write', 'space', '_index')
            box.schema.user.grant('mytestuser', 'read', 'space', '_priv')
            box.schema.user.grant('mytestuser', 'execute', 'universe')

            box.schema.create_space('test', {user = 'mytestuser'})
            box.space.test:create_index('primary', {type = 'hash', parts = {1, 'unsigned'}})

            box.schema.func.create('some_function')
            box.schema.user.grant('mytestuser', 'execute', 'function', 'some_function')

            function some_function()
                return "ok"
            end

            s = box.space.test
            s:insert({1, 'Roxette'})
            s:insert({2, 'Scorpions', 2015})
            s:insert({3, 'Ace of Base', 1993})
        */

        [Fact]
        public async Task ConnectAsGuest()
        {
            var tarantoolClient = TarantoolClient.Create("tarantool-host:3301,tarantool-host:3302,tarantool-host:3303");
            await tarantoolClient.ConnectAsync();
        }

        [Fact]
        public async Task ConnectAsUser()
        {
            var tarantoolClient = TarantoolClient.Create("mytestuser:mytestpass@tarantool-host:3301,tarantool-host:3302,tarantool-host:3303");
            await tarantoolClient.ConnectAsync();
        }

        [Fact]
        public async Task NotConnectAsInvalidUser()
        {
            var tarantoolClient = TarantoolClient.Create(
                    "invaliduser:invalidpass@tarantool-host:3301,tarantool-host:3302,tarantool-host:3303");

            var ex = await Assert.ThrowsAsync<TarantoolException>(() => tarantoolClient.ConnectAsync());
            Assert.Equal("User 'invaliduser' is not found", ex.Message);
        }
    }
}