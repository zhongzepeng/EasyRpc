using System.Reflection;

namespace EasyRpc.Client.Proxy
{
    public interface IInterceptor
    {
        object Intercept(MethodInfo method, object[] parameters);
    }
}
