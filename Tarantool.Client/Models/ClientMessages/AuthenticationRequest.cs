using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class AuthenticationRequest : ClientMessageBase
    {
        public AuthenticationRequest(ulong requestId)
            : base(TarantoolCommand.Auth, requestId)
        {
        }

        public string Username { get; set; }
        public byte[] Scramble { get; set; }

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