using Xunit;

namespace Tarantool.Client
{
    public class AcquiredConnectionShould
    {
        [Fact]
        public void BeAcquiredThenReleased()
        {
            var connection = new TarantoolConnection(null);

            Assert.False(connection.IsAcquired);
            using (new AcquiredConnection(connection))
            {
                Assert.True(connection.IsAcquired);
            }
            Assert.False(connection.IsAcquired);
        }
    }
}
