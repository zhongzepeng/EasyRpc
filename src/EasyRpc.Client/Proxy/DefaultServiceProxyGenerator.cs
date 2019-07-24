using System;
using System.Reflection;

namespace EasyRpc.Client.Proxy
{
    public class DefaultServiceProxyGenerator : IServiceProxyGenerator, IInterceptor
    {
        public object Generate(Type interfaceType)
        {

            return ProxyGenerator.Create(interfaceType, this);
        }

        public TInterfaceType Generate<TInterfaceType>() where TInterfaceType : class
        {
            return ProxyGenerator.Create(typeof(TInterfaceType), this) as TInterfaceType;
        }

        /// <summary>
        /// 在此次处理rpc 数据传输
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Intercept(MethodInfo method, object[] parameters)
        {
            Console.WriteLine($"invoke,method:{method.Name},par:{parameters.Length}");
            return 11111;
        }
    }
}
