﻿using System.Collections.Generic;
using MsgPack;

namespace Tarantool.Client.Models.ClientMessages
{
    public class EvalRequest : ClientMessageBase
    {
        public EvalRequest()
            : base(TarantoolCommand.Eval)
        {
            Args = new List<object>();
        }

        public string Expression { get; set; }

        public IEnumerable<object> Args { get; set; }

        protected override void PackBody(Packer packer)
        {
            packer.PackMapHeader(2);

            packer.Pack((byte)TarantoolKey.Expression);
            packer.Pack(Expression);

            packer.Pack((byte)TarantoolKey.Tuple);
            // ReSharper disable once ExceptionNotDocumented
            packer.PackArray(Args);
        }
    }
}