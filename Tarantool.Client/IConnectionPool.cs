using System.Threading.Tasks;

namespace Tarantool.Client
{
    internal interface IConnectionPool
    {
        Task ConnectAsync();
    }
}