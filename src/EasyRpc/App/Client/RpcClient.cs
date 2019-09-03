using System.Threading.Tasks;
using EasyRpc.App.Proxy;
using Infrastructure.Network.Host;
using Microsoft.Extensions.Logging;
using NetworkClient = Infrastructure.Network.Host.Impl.Client;
namespace EasyRpc.App.Client
{
    public class RpcClient : IHost
    {
        private readonly ILogger<RpcClient> logger;
        private readonly NetworkClient networkClient;
        private readonly IServiceProxyGenerator serviceProxyGenerator;

        public RpcClient(ILogger<RpcClient> logger, NetworkClient client, IServiceProxyGenerator serviceProxyGenerator)
        {
            this.serviceProxyGenerator = serviceProxyGenerator;
            this.logger = logger;
            networkClient = client;
        }
        public async Task StartAsync()
        {
            await networkClient.StartAsync();
        }

        public async Task StopAsync()
        {
            await networkClient.StopAsync();
        }

        public TInterfaceType CreateProxy<TInterfaceType>() where TInterfaceType : class
        {
            return serviceProxyGenerator.Generate<TInterfaceType>();
        }
    }
}