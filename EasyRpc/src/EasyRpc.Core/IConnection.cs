using System.Threading.Tasks;

namespace EasyRpc.Core
{
    public interface IConnection
    {
        Task StartAsync();

        Task StopAsync();
    }
}
