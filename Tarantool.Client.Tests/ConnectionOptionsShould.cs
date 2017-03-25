using Tarantool.Client.Models;
using Xunit;

namespace Tarantool.Client
{
    public class ConnectionOptionsShould
    {
        [Fact]
        public void ParseOneNodeString()
        {
            var opts = new ConnectionOptions("tarantool://tarantool-node");

            Assert.Equal(1, opts.Nodes.Count);
            Assert.Equal("tarantool://tarantool-node:3301/", opts.Nodes[0].ToString());
        }

        [Fact]
        public void ParseThreeNodesString()
        {
            var opts = new ConnectionOptions("tarantool-node1,tarantool-node2,tarantool-node3");

            Assert.Equal(3, opts.Nodes.Count);
            Assert.Equal("tarantool://tarantool-node1:3301/", opts.Nodes[0].ToString());
            Assert.Equal("tarantool://tarantool-node2:3301/", opts.Nodes[1].ToString());
            Assert.Equal("tarantool://tarantool-node3:3301/", opts.Nodes[2].ToString());
        }
    }
}
