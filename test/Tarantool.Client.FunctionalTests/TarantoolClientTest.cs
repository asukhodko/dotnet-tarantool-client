using System.Threading.Tasks;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolClientTest
    {
        public TarantoolClientTest()
        {
            _tarantoolClientGuest = new TarantoolClient(ConnectionStringGuest);
            _tarantoolClient = new TarantoolClient(ConnectionString);
        }

        private readonly TarantoolClient _tarantoolClientGuest;
        private readonly TarantoolClient _tarantoolClient;

        private const string ConnectionStringGuest = "tarantool-host:3301,tarantool-host:3302,tarantool-host:3303";

        private const string ConnectionString =
            "myuser:mypass@tarantool-host:3301,myuser:mypass@tarantool-host:3302,myuser:mypass@tarantool-host:3303";

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