using System.IO;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class AuthenticationBody : ClientMessageBodyBase
    {
        public AuthenticationBody(string username, byte[] scramble)
        {
            Username = username;
            Scramble = scramble;
        }

        public string Username { get; }
        public byte[] Scramble { get; }

        public override void Pack(Packer packer)
        {
            packer.PackMapHeader(2);

            packer.Pack((byte)TarantoolKey.UserName);
            packer.Pack(Username);

            packer.Pack((byte)TarantoolKey.Tuple);
            packer.PackArrayHeader(2);
            packer.Pack("chap-sha1");
            packer.Pack(Scramble);
        }
    }
}