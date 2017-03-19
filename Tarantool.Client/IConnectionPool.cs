using System;
using System.Threading.Tasks;

namespace Tarantool.Client
{
    internal interface IConnectionPool: IDisposable
    {
        Task ConnectAsync();
    }
}