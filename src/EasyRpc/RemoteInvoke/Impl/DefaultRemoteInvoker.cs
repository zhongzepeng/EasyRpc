using System.Threading.Tasks;
using System.Collections.Concurrent;
using NetworkClient = Infrastructure.Network.Host.Impl.Client;
using EasyRpc.RemoteInvoke;
using System;
using Infrastructure.Network.Channel;
using Infrastructure.Network.Codec;
using EasyRpc.Serialization;
using EasyRpc.Exceptions;

namespace EasyRpc.RemoteInvoker.Impl
{
    public class DefaultRemoteInvoker : IRemoteInvoker
    {
        private readonly NetworkClient networkClient;
        private readonly ISerializer serializer;
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<RemoteInvokeTransport<RemoteInvokeResponse>>> resultDictionary
            = new ConcurrentDictionary<Guid, TaskCompletionSource<RemoteInvokeTransport<RemoteInvokeResponse>>>();

        public DefaultRemoteInvoker(NetworkClient client, ISerializer serializer)
        {
            this.serializer = serializer;
            networkClient = client;
            networkClient.OnPackageReceived = OnPackageReceived;
        }

        public async Task<RemoteInvokeResponse> Invoke(RemoteInvokeRequest request)
        {
            try
            {
                var transport = RemoteInvokeTransport<RemoteInvokeRequest>.CreateRequest(request);
                var callbackTask = RegisterCallbackAsync(transport.Id);
                try
                {

                    var package = new Package
                    {
                        Type = PackageType.Transfer,
                        Data = serializer.Serialize(transport)
                    };

                    await networkClient.SendAsync(package);
                }
                catch (Exception ex)
                {
                    throw new EasyRpcException("执行出错1", ex);
                }
                return await callbackTask;
            }
            catch (Exception ex)
            {
                throw new EasyRpcException("执行出错", ex);
            }
        }

        private async Task<RemoteInvokeResponse> RegisterCallbackAsync(Guid guid)
        {
            var task = new TaskCompletionSource<RemoteInvokeTransport<RemoteInvokeResponse>>();
            resultDictionary.TryAdd(guid, task);

            try
            {
                var result = await task.Task;
                return result.Content;
            }
            finally
            {
                resultDictionary.TryRemove(guid, out _);
            }
        }

        private async Task OnPackageReceived(IChannel<Package> channel, Package pacakge)
        {
            if (pacakge.Type != PackageType.Transfer)
            {
                return;
            }

            var data = serializer.Deserialize<RemoteInvokeTransport<RemoteInvokeResponse>>(pacakge.Data);

            if (data == null)
            {
                return;
            }

            await TransportReceived(data);
        }

        private async Task TransportReceived(RemoteInvokeTransport<RemoteInvokeResponse> transport)
        {
            if (!resultDictionary.TryGetValue(transport.Id, out var task))
            {
                return;
            }

            var res = transport.Content;

            if (!res.Success)
            {
                task.TrySetException(new EasyRpcException(res.Message));
            }
            else
            {
                task.SetResult(transport);
            }

            await Task.CompletedTask;
        }
    }
}