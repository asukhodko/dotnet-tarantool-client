using Tarantool.Client.Models;
using Xunit;

namespace Tarantool.Client
{
    public class ConnectionPool_GetPoolShould
    {
        [Fact]
        public void ReturnSamePoolForSameConnectionString()
        {
            var pool1 = ConnectionPool.GetPool(new ConnectionOptions("tarantool-host1,tarantool-host2,tarantool-host3"));
            var pool2 = ConnectionPool.GetPool(new ConnectionOptions("tarantool-host1,tarantool-host2,tarantool-host3"));

            Assert.Equal(pool1, pool2);
        }

        [Fact]
        public void ReturnNotSamePoolForDifferentConnectionString()
        {
            var pool1 = ConnectionPool.GetPool(new ConnectionOptions("tarantool-host1,tarantool-host2,tarantool-host3"));
            var pool2 = ConnectionPool.GetPool(new ConnectionOptions("tarantool-hostA,tarantool-hostB,tarantool-hostC"));

            Assert.NotEqual(pool1, pool2);
        }
    }
}
