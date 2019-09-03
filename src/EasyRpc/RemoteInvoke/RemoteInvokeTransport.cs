using System;

namespace EasyRpc.RemoteInvoke
{
    /// <summary>
    /// 传输对象
    /// </summary>
    public class RemoteInvokeTransport<T> where T : class
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public T Content { get; set; }

        public static RemoteInvokeTransport<RemoteInvokeRequest> CreateRequest(RemoteInvokeRequest request)
        {
            return new RemoteInvokeTransport<RemoteInvokeRequest>
            {
                Content = request
            };
        }

        public static RemoteInvokeTransport<RemoteInvokeResponse> CreateResponse(Guid id, RemoteInvokeResponse response)
        {
            return new RemoteInvokeTransport<RemoteInvokeResponse>
            {
                Content = response,
                Id = id
            };
        }

    }
}