using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class EvalRequest : ClientMessageBase
    {
        public EvalRequest(ulong requestId)
            : base(TarantoolCommand.Eval, requestId)
        {
        }

        public string Expression { get; set; }

        public long[] Args { get; set; }

        protected override void PackBody(Packer packer)
        {
            if (Args == null)
                Args = new long[0];

            packer.PackMapHeader(2);

            packer.Pack((byte)TarantoolKey.Expression);
            packer.Pack(Expression);

            packer.Pack((byte)TarantoolKey.Tuple);
            // ReSharper disable once ExceptionNotDocumented
            packer.PackArrayHeader(Args.Length);
            foreach (var arg in Args)
            {
                packer.Pack(arg);
            }
        }
    }
}