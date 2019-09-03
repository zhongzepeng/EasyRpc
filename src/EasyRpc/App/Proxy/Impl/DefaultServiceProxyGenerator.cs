using System;
using System.Reflection;
using EasyRpc.RemoteInvoke;
using EasyRpc.RemoteInvoker;
using EasyRpc.Serialization;
using Infrastructure.Basics.Async;
using Microsoft.Extensions.Logging;
using NetworkClient = Infrastructure.Network.Host.Impl.Client;

namespace EasyRpc.App.Proxy.Impl
{
    public class DefaultServiceProxyGenerator : IServiceProxyGenerator, IInterceptor
    {
        private readonly ILogger<DefaultServiceProxyGenerator> logger;
        private readonly IRemoteInvoker remoteInvoker;
        public DefaultServiceProxyGenerator(ILogger<DefaultServiceProxyGenerator> logger
        , IRemoteInvoker remoteInvoker)
        {
            this.remoteInvoker = remoteInvoker;
            this.logger = logger;
        }

        public object Generate(Type interfaceType)
        {
            return ProxyGenerator.Create(interfaceType, this);
        }

        public TInterfaceType Generate<TInterfaceType>() where TInterfaceType : class
        {
            return ProxyGenerator.Create(typeof(TInterfaceType), this) as TInterfaceType;
        }

        public object InterceptAsync(MethodInfo method, object[] parameters)
        {
            logger.LogInformation($"执行方法,{method.Name}");
            var request = RemoteInvokeRequest.Create(method, parameters);

            // var response = remoteInvoker.Invoke(request).GetAwaiter().GetResult();
            var response = AsyncHelper.RunSync(() => remoteInvoker.Invoke(request));

            logger.LogInformation($"方法执行结果：{response.Success}");
            return null;
        }
    }
}