using System.Threading.Tasks;
using System;
using System.Reflection;
using EasyRpc.RemoteInvoke;
using EasyRpc.RemoteInvoker;
using EasyRpc.Serialization;
using Infrastructure.Basics.Async;
using Microsoft.Extensions.Logging;

namespace EasyRpc.App.Proxy.Impl
{
    public class DefaultServiceProxyGenerator : IServiceProxyGenerator, IInterceptor
    {
        private readonly ILogger<DefaultServiceProxyGenerator> logger;
        private readonly IRemoteInvoker remoteInvoker;
        private readonly ISerializer serializer;
        public DefaultServiceProxyGenerator(ILogger<DefaultServiceProxyGenerator> logger
        , IRemoteInvoker remoteInvoker
        , ISerializer serializer)
        {
            this.remoteInvoker = remoteInvoker;
            this.logger = logger;
            this.serializer = serializer;
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
            var request = RemoteInvokeRequest.Create(method, parameters);

            var response = AsyncHelper.RunSync(() => remoteInvoker.Invoke(request));

            if (method.ReturnType == typeof(void))
            {
                return null;
            }

            if (method.ReturnType == typeof(Task))
            {
                return Task.CompletedTask;
            }

            if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var returnValueType = method.ReturnType.GetGenericArguments()[0];

                var resultValue = serializer.Deserialize(response.Data, returnValueType);

                return Task.FromResult(resultValue);
            }
            else
            {
                return serializer.Deserialize(response.Data, method.ReturnType);
            }
        }
    }
}