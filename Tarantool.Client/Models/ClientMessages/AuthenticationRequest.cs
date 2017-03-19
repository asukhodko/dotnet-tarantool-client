using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class AuthenticationRequest : ClientMessageBase
    {
        public AuthenticationRequest(ulong requestId, string username, byte[] scramble)
            : base(TarantoolCommand.Auth, requestId)
        {
            Username = username;
            Scramble = scramble;
        }

        public string Username { get; }
        public byte[] Scramble { get; }

        protected override void PackBody(Packer packer)
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