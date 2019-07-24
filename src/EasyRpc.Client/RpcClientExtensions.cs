using EasyRpc.Client.Proxy;
using EasyRpc.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace EasyRpc.Client
{
    public static class RpcClientExtensions
    {
        public static void AddRpcClient(this ServiceCollection service)
        {
            RegisterAllRpcApi(service);
        }

        private static void RegisterAllRpcApi(ServiceCollection service)
        {
            var serviceProxyGenerator = new DefaultServiceProxyGenerator();

            var apis = Assembly
                 .GetEntryAssembly()
                 .GetTypes()
                 .Where(type => type.IsInterface && type.IsDefined(typeof(RpcApiAttribute)) && type.IsPublic);

            foreach (var api in apis)
            {
                service.AddSingleton(api, serviceProxyGenerator.Generate(api));
            }
        }
    }
}
