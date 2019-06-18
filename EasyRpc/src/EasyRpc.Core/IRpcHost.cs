using System.Threading.Tasks;

namespace EasyRpc.Core
{
    public interface IRpcHost
    {
        Task StartAsync();

        Task StopAsync();
    }
}
