using Tarantool.Client.Models;
using Xunit;

namespace Tarantool.Client
{
    public class ConnectionOptions_ToStringShould
    {
        [Fact]
        public void GenerateConnectionString()
        {
            var opts = new ConnectionOptions("tarantool-node1,tarantool-node2,tarantool-node3:3303");

            var connectionString = opts.ToString();

            Assert.Equal("tarantool://tarantool-node1:3301,tarantool-node2:3301,tarantool-node3:3303", connectionString);
        }
    }
}
