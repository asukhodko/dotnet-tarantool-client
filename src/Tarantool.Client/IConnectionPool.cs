using System.Threading.Tasks;

namespace Tarantool.Client
{
    public interface IConnectionPool
    {
        Task ConnectAsync();
    }
}