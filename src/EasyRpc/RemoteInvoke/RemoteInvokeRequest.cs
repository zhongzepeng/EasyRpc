using System.Linq;
using System.Reflection;
using EasyRpc.Extensions;
using Infrastructure.Basics;

namespace EasyRpc.RemoteInvoke
{
    public class RemoteInvokeRequest
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        /// <value></value>
        public string FullTypeName { get; set; }
        /// <summary>
        /// 参数名列表
        /// </summary>
        /// <value></value>
        public string ParameterIdentity { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        /// <value></value>
        public object[] Parameters { get; set; }

        public string MethodName
        {
            get
            {
                var index = ParameterIdentity.IndexOf("_");
                return ParameterIdentity.Substring(0, index);
            }
        }

        public override string ToString()
        {
            return $"{FullTypeName}_{string.Join(',', ParameterIdentity)}_{Parameters.Length}";
        }

        public static RemoteInvokeRequest Create(MethodInfo methodInfo, object[] parameters)
        {
            var type = methodInfo.DeclaringType;
            Checker.NotNull(type, nameof(methodInfo.DeclaringType));

            return new RemoteInvokeRequest
            {
                FullTypeName = type.FullName,
                ParameterIdentity = methodInfo.GetMethodIdentity(),
                Parameters = parameters
            };
        }
    }
}