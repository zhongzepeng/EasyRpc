using System;
using EasyRpc.App.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EasyRpc.Extensions;
using EasyRpc.App.Client;
using EasyRpc.Test.Common;

namespace EasyRpc.Test.Client
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            var configuration = BuildConfiguration();
            serviceCollection.AddRpcClient(configuration);
            serviceCollection.AddLogging(configure => configure.AddConsole());

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var client = serviceProvider.GetService<RpcClient>();
            await client.StartAsync();

            var userService = client.CreateProxy<IUserService>();
            // userService.GetUserNameById(new TestParam { Id = 1.ToString() });
            // await userService.GetUserNameByIdAsync(1);
            userService.GetUser<TestParam>(1);
            var result = userService.GetUserNameById(new TestParam { Id = 1.ToString() });

            Console.ReadLine();
        }

        static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            return builder.Build();
        }
    }
}
