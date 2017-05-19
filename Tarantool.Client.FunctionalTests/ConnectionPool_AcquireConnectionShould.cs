using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tarantool.Client.Models;
using Tarantool.Client.FunctionalTests.PrivateAccessors;
using Tarantool.Client.Models.ClientMessages;

using Xunit;

namespace Tarantool.Client
{
    [Collection("Tarantool database collection")]
    public class ConnectionPool_AcquireConnectionShould
    {
        [Fact]
        public async Task CreateFirstConnection()
        {
            var pool =
                (ConnectionPool)
                ConnectionPool.GetPool(new ConnectionOptions("127.0.0.1:3301"));
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
                ConnectionPool.GetPool(new ConnectionOptions("127.0.0.2:3301"));
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
                ConnectionPool.GetPool(new ConnectionOptions("127.0.0.3:3301"));
            using (var connection = await pool.AcquireConnectionAsync())
            {
                await connection.RequestAsync(new EvalRequest { Expression = "return 0" }, CancellationToken.None);
            }
            Assert.Equal(1, pool._connections().Count);

            foreach (var dummy in Enumerable.Repeat(0, 100))
            {
                using (var connection = await pool.AcquireConnectionAsync())
                {
                    await connection.RequestAsync(new EvalRequest { Expression = "return 0" }, CancellationToken.None);

                    Assert.Equal(1, pool._connections().Count);
                }
            }

            Assert.Equal(1, pool._connections().Count);
        }
    }
}