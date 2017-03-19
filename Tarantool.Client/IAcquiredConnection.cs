using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;

namespace Tarantool.Client
{
    internal interface IAcquiredConnection : IDisposable
    {
        Task<IList<MessagePackObject>> EvalAsync(string expression, long[] args);
    }
}