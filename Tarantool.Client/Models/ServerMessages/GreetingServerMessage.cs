using System;
using System.Text;

namespace Tarantool.Client.Models.ServerMessages
{
    public class GreetingServerMessage
    {
        public GreetingServerMessage(byte[] bytes)
        {
            ServerVersion = Encoding.ASCII.GetString(bytes, 0, 64);
            SaltString = Encoding.ASCII.GetString(bytes, 64, 44);
            Salt = Convert.FromBase64String(SaltString);
        }

        public byte[] Salt { get; }

        public string ServerVersion { get; }

        public string SaltString { get; }
    }
}