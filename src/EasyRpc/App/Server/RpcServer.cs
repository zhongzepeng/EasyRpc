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
            logger.LogInformation($"服务端接受到一个数据包：{request.Content}");
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

            var result = method.Invoke(instance, parameters.ToArray());

            //异步返回
            if (result.GetType() == typeof(Task) || result.GetType().GetGenericTypeDefinition() == typeof(Task<>))
            {
                var task = (Task)result;
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result")?.GetValue(task);
                logger.LogInformation(resultProperty.ToString() + "1111");
            }

            logger.LogInformation($"{result.GetType()}");

            await SendAsync(channel, RemoteInvokeResponse.CreateSuccess(null), id);
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