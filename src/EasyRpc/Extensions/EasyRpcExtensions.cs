using System.Reflection;
using EasyRpc.App.Client;
using EasyRpc.App.Proxy;
using EasyRpc.App.Proxy.Impl;
using EasyRpc.App.Server;
using EasyRpc.RemoteInvoker;
using EasyRpc.RemoteInvoker.Impl;
using EasyRpc.Serialization;
using EasyRpc.Serialization.Impl;
using Infrastructure.Network.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using EasyRpc.Services;
using System;

namespace EasyRpc.Extensions
{
    public static class EasyRpcExtensions
    {
        public static void AddRpcServer(this IServiceCollection serviceCollection, IConfiguration configuration, Assembly assembly)
        {
            RegisterIService(assembly);
            serviceCollection.AddServer(configuration);
            serviceCollection.Configure<RpcServerOptions>(configuration.GetSection("EasyRpcServer"));
            serviceCollection.AddSingleton<ISerializer, DefaultJsonSerializer>();
            serviceCollection.AddSingleton<RpcServer>();
        }

        private static void RegisterIService(Assembly assembly)
        {
            var serviceCollection = new ServiceCollection();
            var classTypes = assembly
            .GetTypes()
            .Where(type => type.IsClass && type.IsPublic && typeof(IRpcService).IsAssignableFrom(type));

            foreach (var classType in classTypes)
            {
                var interfaceType = classType.GetInterfaces().FirstOrDefault();
                serviceCollection.AddSingleton(interfaceType, classType);
                Console.WriteLine($"注册服务：{interfaceType.FullName},{classType.Name}");
            }
            var provider = serviceCollection.BuildServiceProvider();

            RpcServiceLocator.Init(provider, assembly);
        }

        public static void AddRpcClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddClient(configuration);
            serviceCollection.AddSingleton<ISerializer, DefaultJsonSerializer>();
            serviceCollection.AddSingleton<IServiceProxyGenerator, DefaultServiceProxyGenerator>();
            serviceCollection.AddSingleton<IRemoteInvoker, DefaultRemoteInvoker>();
            serviceCollection.AddSingleton<RpcClient>();
        }

        internal static string GetMethodIdentity(this MethodInfo methodInfo)
        {
            var parameters = string.Join("_", methodInfo.GetParameters().Select(x => $"{x.Name}_{x.ParameterType.Name}"));
            return $"{methodInfo.Name}_{parameters}";
        }
    }
}