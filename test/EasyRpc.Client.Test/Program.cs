using EasyRpc.Client.Proxy;
using System;

namespace EasyRpc.Client.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new DefaultServiceProxyGenerator();

            var instance = generator.Generate<ITestService>();

            instance.TestMethod();
            instance.TestMethod1(11);
            instance.TestMethod2("123412", "21321");

            Console.ReadKey();
        }
    }
}
