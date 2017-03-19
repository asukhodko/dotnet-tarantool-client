using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class EvalRequest : ClientMessageBase
    {
        public EvalRequest(ulong requestId, string expression, long[] args)
            : base(TarantoolCommand.Eval, requestId)
        {
            Expression = expression;
            if (args == null)
                args = new long[0];
            Args = args;
        }

        public long[] Args { get; }

        public string Expression { get; }

        protected override void PackBody(Packer packer)
        {
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