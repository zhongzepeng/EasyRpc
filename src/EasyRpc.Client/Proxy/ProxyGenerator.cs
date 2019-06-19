using System;
using System.Reflection;

namespace EasyRpc.Client.Proxy
{
    public class ProxyGenerator : DispatchProxy
    {
        private IInterceptor interceptor { get; set; }

        public static object Create(Type targetType, IInterceptor interceptor)
        {
            object proxy = GetProxy(targetType);
            MethodInfo method = typeof(ProxyGenerator).GetMethod(nameof(CreateInstance), BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(IInterceptor) }, null);
            method.Invoke(proxy, new object[] { interceptor });
            return proxy;
        }

        public static object Create(Type targetType, Type interceptorType, params object[] parameters)
        {
            object proxy = GetProxy(targetType);
            MethodInfo method = typeof(ProxyGenerator).GetMethod(nameof(CreateInstance), BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(Type), typeof(object[]) }, null);
            method.Invoke(proxy, new object[] { interceptorType, parameters });
            return proxy;
        }


        public static TTarget Create<TTarget, TInterceptor>(params object[] parameters) where TInterceptor : IInterceptor
        {
            object proxy = GetProxy(typeof(TTarget));
            MethodInfo method = typeof(ProxyGenerator).GetMethod(nameof(CreateInstance), BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(Type), typeof(object[]) }, null);
            method.Invoke(proxy, new object[] { typeof(TInterceptor), parameters });
            return (TTarget)proxy;
        }

        private static object GetProxy(Type targetType)
        {
            MethodInfo method = typeof(DispatchProxy).GetMethod(nameof(DispatchProxy.Create), new Type[] { });
            method = method.MakeGenericMethod(targetType, typeof(ProxyGenerator));
            return method.Invoke(null, null);
        }

        private void CreateInstance(Type interceptorType, object[] parameters)
        {
            interceptor = (IInterceptor)Activator.CreateInstance(interceptorType, parameters);
        }

        private void CreateInstance(IInterceptor interceptor)
        {
            this.interceptor = interceptor;
        }

        protected override object Invoke(MethodInfo method, object[] parameters)
        {
            return interceptor.Intercept(method, parameters);
        }
    }
}
