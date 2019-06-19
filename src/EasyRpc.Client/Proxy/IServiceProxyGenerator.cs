using System;

namespace EasyRpc.Client.Proxy
{
    public interface IServiceProxyGenerator
    {
        object Generate(Type interfaceType);

        TInterfaceType Generate<TInterfaceType>() where TInterfaceType : class;
    }
}
