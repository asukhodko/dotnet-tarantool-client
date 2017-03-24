using System.Collections.Generic;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class CallRequest : ClientMessageBase
    {
        public CallRequest()
            : base(TarantoolCommand.Call)
        {
            Args = new List<object>();
        }

        public string FunctionName { get; set; }

        public IEnumerable<object> Args { get; set; }

        protected override void PackBody(Packer packer)
        {
            packer.PackMapHeader(2);

            packer.Pack((byte)TarantoolKey.Function);
            packer.Pack(FunctionName);

            packer.Pack((byte)TarantoolKey.Tuple);
            // ReSharper disable once ExceptionNotDocumented
            packer.PackArray(Args);
        }
    }
}