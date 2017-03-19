using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;

namespace Tarantool.Client
{
    internal interface IConnectionPool: IDisposable
    {
        Task ConnectAsync();

        Task<IList<MessagePackObject>> EvalAsync(string expression, long[] args);
    }
}