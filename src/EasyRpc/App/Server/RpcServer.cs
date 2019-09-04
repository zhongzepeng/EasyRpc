using System.Reflection;
using System.Linq;
using System;
using System.Threading.Tasks;
using EasyRpc.RemoteInvoke;
using EasyRpc.Serialization;
using Infrastructure.Network.Channel;
using Infrastructure.Network.Codec;
using Infrastructure.Network.Host;
using Microsoft.Extensions.Logging;
using NetworkServer = Infrastructure.Network.Host.Impl.Server;
using EasyRpc.Extensions;
using System.Collections.Generic;

namespace EasyRpc.App.Server
{
    public class RpcServer : IHost
    {
        private readonly ILogger<RpcServer> logger;
        private readonly ISerializer serializer;
        private readonly NetworkServer networkServer;

        public RpcServer(ILogger<RpcServer> logger, NetworkServer server, ISerializer serializer)
        {
            this.logger = logger;
            networkServer = server;
            networkServer.OnPackageReceived = OnPackageReceived;
            this.serializer = serializer;
        }
        public async Task StartAsync()
        {
            await networkServer.StartAsync();
        }

        public async Task StopAsync()
        {
            await networkServer.StopAsync();
        }

        private async Task OnPackageReceived(IChannel<Package> channel, Package package)
        {
            if (package.Type != PackageType.Transfer)
            {
                return;
            }

            var request = serializer.Deserialize<RemoteInvokeTransport<RemoteInvokeRequest>>(package.Data);

            try
            {
                await OnRemoteInvokeRequest(channel, request);
            }
            catch (System.Exception ex)
            {
                logger.LogError($"OnRemoteInvokeRequest{ex.ToString()}");
                throw ex;
            }

        }

        private async Task OnRemoteInvokeRequest(IChannel<Package> channel, RemoteInvokeTransport<RemoteInvokeRequest> request)
        {
            logger.LogDebug($"服务端接受到一个数据包：{request.Content}");
            var id = request.Id;
            var invokeRequest = request.Content;

            var type = RpcServiceLocator.ServiceAssembly.GetType(invokeRequest.FullTypeName);
            if (type == null)
            {
                await SendAsync(channel, RemoteInvokeResponse.CreateFail("没有发现对应服务"), id);
                return;
            }

            var instance = RpcServiceLocator.Instance.GetService(type);

            if (instance == null)
            {
                await SendAsync(channel, RemoteInvokeResponse.CreateFail("没有找到实现类"), id);
                return;
            }
            var method = type.GetMethods().SingleOrDefault(x =>
            {
                if (x.Name != invokeRequest.MethodName)
                {
                    return false;
                }
                var identity = x.GetMethodIdentity();
                return identity == invokeRequest.ParameterIdentity;

            });

            if (method == null)
            {
                await SendAsync(channel, RemoteInvokeResponse.CreateFail("没有找到具体方法"), id);
                return;
            }

            var parameters = new List<object>();
            var methodParameters = method.GetParameters();

            for (int i = 0; i < methodParameters.Length; i++)
            {
                var data = serializer.Serialize(invokeRequest.Parameters[i]);
                var param = serializer.Deserialize(data, methodParameters[i].ParameterType);
                parameters.Add(param);
            }

            if (method.IsGenericMethod)
            {
                await SendAsync(channel, RemoteInvokeResponse.CreateFail("不支持泛型方法"), id);
                return;
            }

            var result = await InvokeMethod(method, instance, parameters.ToArray());

            await SendAsync(channel, result, id);
        }

        private async Task<RemoteInvokeResponse> InvokeMethod(MethodInfo methodInfo, Object instance, object[] parameters)
        {
            var result = methodInfo.Invoke(instance, parameters.ToArray());

            if (methodInfo.ReturnType == typeof(void) || methodInfo.ReturnType == typeof(Task))
            {
                return RemoteInvokeResponse.CreateSuccess(null);
            }
            object resultObj;
            if (result.GetType().IsGenericType && result.GetType().GetGenericTypeDefinition() == typeof(Task<>))
            {
                if (result.GetType().GetGenericArguments().Count() > 1)
                {
                    return RemoteInvokeResponse.CreateFail("不支持多个泛型参数的方法");
                }

                var task = (Task)result;
                await task.ConfigureAwait(false);
                resultObj = task.GetType().GetProperty("Result").GetValue(task);
            }
            else
            {
                resultObj = result;
            }
            if (resultObj == null)
            {
                return RemoteInvokeResponse.CreateSuccess(null);
            }
            else
            {
                return RemoteInvokeResponse.CreateSuccess(serializer.Serialize(resultObj));
            }
        }

        private async Task SendAsync(IChannel<Package> channel, RemoteInvokeResponse res, Guid id)
        {
            var response = RemoteInvokeTransport<RemoteInvokeResponse>.CreateResponse(id, res);
            var resData = serializer.Serialize(response);
            var package = new Package
            {
                Type = PackageType.Transfer,
                Data = resData
            };
            await channel.SendAsync(package);
        }

    }
}