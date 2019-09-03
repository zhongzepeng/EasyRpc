using Microsoft.Extensions.Options;

namespace EasyRpc.App.Server
{
    public class RpcServerOptions : IOptions<RpcServerOptions>
    {
        public RpcServerOptions Value => this;

    }
}