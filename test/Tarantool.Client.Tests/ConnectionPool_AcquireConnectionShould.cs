using Tarantool.Client.Models;
using Tarantool.Client.Tests.PrivateAccessors;
using Xunit;

namespace Tarantool.Client
{
    public class ConnectionPool_AcquireConnectionShould
    {
        [Fact]
        public void CreateFirstConnection()
        {
            var pool = (ConnectionPool)ConnectionPool.GetPool(new ConnectionOptions("tarantool-host-ForAcquireConnectionTest1"));
            Assert.Equal(0, pool._connections().Count);

            using (var connection = pool.AcquireConnection())
            {

                Assert.NotNull(connection);
                Assert.Equal(1, pool._connections().Count);
            }
            Assert.Equal(1, pool._connections().Count);
        }

        [Fact]
        public void ReuseConnection()
        {
            var pool = (ConnectionPool)ConnectionPool.GetPool(new ConnectionOptions("tarantool-host-ForAcquireConnectionTest2"));
            using (pool.AcquireConnection()) { }
            Assert.Equal(1, pool._connections().Count);

            using (var connection = pool.AcquireConnection())
            {

                Assert.NotNull(connection);
                Assert.Equal(1, pool._connections().Count);
            }
            Assert.Equal(1, pool._connections().Count);
        }

        [Fact]
        public void CreateNewConnectionWhenAcquired()
        {
            var pool = (ConnectionPool)ConnectionPool.GetPool(new ConnectionOptions("tarantool-host-ForAcquireConnectionTest3"));
            using (pool.AcquireConnection())
            {
                Assert.Equal(1, pool._connections().Count);

                using (var connection = pool.AcquireConnection())
                {

                    Assert.NotNull(connection);
                    Assert.Equal(2, pool._connections().Count);
                }
            }
            Assert.Equal(2, pool._connections().Count);
        }
    }
}
