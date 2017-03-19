using System.Threading.Tasks;
using Tarantool.Client.Models;
using Tarantool.Client.Tests.PrivateAccessors;
using Xunit;

namespace Tarantool.Client
{
    public class ConnectionPool_AcquireConnectionShould
    {
        [Fact]
        public async Task CreateFirstConnection()
        {
            var pool =
                (ConnectionPool)
                ConnectionPool.GetPool(new ConnectionOptions("tarantool-host:3301"));
            Assert.Equal(0, pool._connections().Count);

            using (var connection = await pool.AcquireConnectionAsync())
            {
                Assert.NotNull(connection);
                Assert.Equal(1, pool._connections().Count);
            }
            Assert.Equal(1, pool._connections().Count);
        }

        [Fact]
        public async Task CreateNewConnectionWhenAcquired()
        {
            var pool =
                (ConnectionPool)
                ConnectionPool.GetPool(new ConnectionOptions("tarantool-host:3302"));
            using (await pool.AcquireConnectionAsync())
            {
                Assert.Equal(1, pool._connections().Count);

                using (var connection = await pool.AcquireConnectionAsync())
                {
                    Assert.NotNull(connection);
                    Assert.Equal(2, pool._connections().Count);
                }
            }
            Assert.Equal(2, pool._connections().Count);
        }

        [Fact]
        public async Task ReuseConnection()
        {
            var pool =
                (ConnectionPool)
                ConnectionPool.GetPool(new ConnectionOptions("tarantool-host:3303"));
            using (await pool.AcquireConnectionAsync())
            {
            }
            Assert.Equal(1, pool._connections().Count);

            using (var connection = await pool.AcquireConnectionAsync())
            {
                Assert.NotNull(connection);
                Assert.Equal(1, pool._connections().Count);
            }
            Assert.Equal(1, pool._connections().Count);
        }
    }
}