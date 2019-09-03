using System.Reflection;
using System;
using Infrastructure.Basics;

namespace EasyRpc.App.Server
{
    internal class RpcServiceLocator
    {
        public static IServiceProvider Instance { get; private set; }

        public static Assembly ServiceAssembly { get; private set; }

        public static void Init(IServiceProvider privider, Assembly assembly)
        {
            ServiceAssembly = assembly;
            Instance = privider;
        }

        public static T GetService<T>() where T : class
        {
            return GetService(typeof(T)) as T;
        }

        public static object GetService(Type type)
        {
            Checker.NotNull(Instance, "请调用 Init 进行初始化");

            return Instance.GetService(type);
        }
    }
}