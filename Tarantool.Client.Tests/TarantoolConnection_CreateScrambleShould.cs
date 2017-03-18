using Tarantool.Client.Models;
using Tarantool.Client.Tests.PrivateAccessors;
using Xunit;

namespace Tarantool.Client
{
    public class TarantoolConnection_CreateScrambleShould
    {
        [Fact]
        public void MatchExample1()
        {
            var password = "some password 1";
            var salt = new byte[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31
            };
            Assert.Equal(32, salt.Length);
            var connection = new TarantoolConnection(null);
            var expectedScramble = new byte[] {
                227, 42, 15, 165, 59, 71, 178, 220, 12, 219,
                71, 208, 188, 118, 221, 79, 144, 77, 181, 144};

            var scramble = connection.CreateScramble(password, salt);

            Assert.Equal(expectedScramble, scramble);
        }

        [Fact]
        public void MatchExample2()
        {
            var password = "some password 2";
            var salt = new byte[]
            {
                10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31
            };
            Assert.Equal(32, salt.Length);
            var connection = new TarantoolConnection(null);
            var expectedScramble = new byte[] {
                232, 180, 54, 215, 151, 79, 146, 37, 53, 76,
                68, 113, 18, 155, 54, 76, 234, 152, 137, 163};

            var scramble = connection.CreateScramble(password, salt);

            Assert.Equal(expectedScramble, scramble);
        }
    }
}