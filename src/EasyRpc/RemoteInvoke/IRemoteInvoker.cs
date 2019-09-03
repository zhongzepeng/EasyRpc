using System.Threading.Tasks;
using EasyRpc.RemoteInvoke;

namespace EasyRpc.RemoteInvoker
{
    public interface IRemoteInvoker
    {
        Task<RemoteInvokeResponse> Invoke(RemoteInvokeRequest request);
    }
}