using System.Reflection;
using System;
using EasyRpc.App.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EasyRpc.Extensions;
using EasyRpc.Test.Common;

namespace EasyRpc.Test.Server
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            var configuration = BuildConfiguration();
            serviceCollection.AddLogging(configure => configure.AddConsole());

            //获取需要注册的服务所在的程序集
            var serviceAssembly = Assembly.GetAssembly(typeof(IUserService));
            serviceCollection.AddRpcServer(configuration, serviceAssembly);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var server = serviceProvider.GetService<RpcServer>();
            await server.StartAsync();


            Console.ReadKey();
        }

        static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            return builder.Build();
        }
    }
}
