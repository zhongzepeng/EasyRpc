using System.Reflection;
namespace EasyRpc.App.Proxy
{
    public interface IInterceptor
    {
        object InterceptAsync(MethodInfo method, object[] parameters);
    }
}