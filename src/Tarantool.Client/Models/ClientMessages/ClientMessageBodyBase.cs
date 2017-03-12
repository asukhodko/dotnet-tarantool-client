using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public abstract class ClientMessageBodyBase
    {
        public abstract void Pack(Packer packer);
    }
}
