using System.Reflection;

namespace Tarantool.Client.Tests.PrivateAccessors
{
    public static class TarantoolConnectionPrivateAccessor
    {
        internal static byte[] CreateScramble(this TarantoolConnection pool, string password, byte[] salt)
        {
            var method = typeof(TarantoolConnection).GetMethod("CreateScramble", BindingFlags.NonPublic | BindingFlags.Static);
            return (byte[])method.Invoke(pool, new object[] {password, salt});
        }
    }
}