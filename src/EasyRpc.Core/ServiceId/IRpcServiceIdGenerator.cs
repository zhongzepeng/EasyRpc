using System.Reflection;

namespace EasyRpc.Core.ServiceId
{
    public interface IRpcServiceIdGenerator
    {
        string Generate(MethodInfo methodInfo);
    }
}
