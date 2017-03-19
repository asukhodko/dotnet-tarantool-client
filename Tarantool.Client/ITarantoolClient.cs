using System.Collections.Generic;
using System.Threading.Tasks;
using MsgPack;

namespace Tarantool.Client
{
    public interface ITarantoolClient
    {
        Task ConnectAsync();

        Task<IList<MessagePackObject>> EvalAsync(string expression);

        Task<IList<MessagePackObject>> EvalAsync(string expression, long[] args);
    }
}