using System;
using NetworkClient = Infrastructure.Network.Host.Impl.Client;

namespace EasyRpc.App.Proxy
{
    public interface IServiceProxyGenerator
    {
        object Generate(Type interfaceType);

        TInterfaceType Generate<TInterfaceType>() where TInterfaceType : class;
    }
}