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

        public TarantoolClientShould()
        {
            _tarantoolClientGuest = new TarantoolClient(ConnectionStringGuest);
            _tarantoolClient = new TarantoolClient(ConnectionString);
        }

        private readonly TarantoolClient _tarantoolClientGuest;
        private readonly TarantoolClient _tarantoolClient;

        private const string ConnectionStringGuest = "guest@tarantool-host:3301,tarantool-host:3302,tarantool-host:3303";

        private const string ConnectionString =
            "mytestuser:mytestpass@tarantool-host:3301,tarantool-host:3302,tarantool-host:3303";

        [Fact]
        public async Task ConnectAsGuest()
        {
            await _tarantoolClientGuest.ConnectAsync();
        }

        [Fact]
        public async Task ConnectAsUser()
        {
            await _tarantoolClient.ConnectAsync();
        }
    }
}