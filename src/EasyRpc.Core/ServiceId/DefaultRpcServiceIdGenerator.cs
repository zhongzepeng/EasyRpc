using EasyRpc.Core.Common;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Reflection;

namespace EasyRpc.Core.ServiceId
{
    internal class DefaultRpcServiceIdGenerator : IRpcServiceIdGenerator
    {
        private readonly ILogger<DefaultRpcServiceIdGenerator> logger;

        public DefaultRpcServiceIdGenerator(ILogger<DefaultRpcServiceIdGenerator> logger)
        {
            this.logger = logger;
        }

        public string Generate(MethodInfo methodInfo)
        {
            Checker.NotNull(methodInfo, nameof(methodInfo));

            var declaringType = methodInfo.DeclaringType;

            Checker.NotNull(declaringType, "DeclaringType can't be null");

            var id = $"{declaringType.FullName}.{methodInfo.Name}";

            var parameters = methodInfo.GetParameters();

            if (parameters.Any())
            {
                id += $"_{string.Join("_", parameters.Select(p => p.ParameterType.FullName))}";
            }

            logger.LogDebug($"generate serviceId:{id}");

            return id;
        }
    }
}
