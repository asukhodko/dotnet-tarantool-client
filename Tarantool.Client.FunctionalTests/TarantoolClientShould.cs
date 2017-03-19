using System;
using System.Threading.Tasks;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClientShould
    {
        /*
          Tarantool user setup:
            box.schema.create_space('test')
            box.schema.user.create('mytestuser', { password = 'mytestpass' })
            box.schema.user.grant('mytestuser', 'read,write,execute', 'space', 'test')
        */

        [Fact]
        public async Task ConnectAsGuest()
        {
            var tarantoolClient = new TarantoolClient("tarantool-host:3301,tarantool-host:3302,tarantool-host:3303");
            await tarantoolClient.ConnectAsync();
        }

        [Fact]
        public async Task ConnectAsUser()
        {
            var tarantoolClient =
                new TarantoolClient("mytestuser:mytestpass@tarantool-host:3301,tarantool-host:3302,tarantool-host:3303");
            await tarantoolClient.ConnectAsync();
        }

        [Fact]
        public async Task NotConnectAsInvalidUser()
        {
            var tarantoolClient =
                new TarantoolClient(
                    "invaliduser:invalidpass@tarantool-host:3301,tarantool-host:3302,tarantool-host:3303");

            var ex = await Assert.ThrowsAsync<TarantoolException>(() => tarantoolClient.ConnectAsync());
            Assert.Equal("User 'invaliduser' is not found", ex.Message);
        }
    }
}