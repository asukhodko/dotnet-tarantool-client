using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class AuthenticationRequest : ClientMessageBase
    {
        public AuthenticationRequest()
            : base(TarantoolCommand.Auth)
        {
        }

        public string Username { get; set; }
        public byte[] Scramble { get; set; }

        public override void PackToMessage(Packer packer, PackingOptions options)
        {
            PackHeader(packer);

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