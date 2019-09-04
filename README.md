# EasyRpc

实现简单的Rpc远程调用

+ 使用DispatchProxy生成动态代理
+ 基于System.IO.Pipelines 实现命令传输
+ 使用System.Text.JSON 作为序列化工具
+ 基于 Microsoft.Extensions.DependencyInjection 的简单服务注册和服务查找

## 使用方法
+ 服务端
   + ```    //获取需要注册的服务所在的程序集
            var serviceAssembly = Assembly.GetAssembly(typeof(IUserService));
            serviceCollection.AddRpcServer(configuration, serviceAssembly);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var server = serviceProvider.GetService<RpcServer>();
            await server.StartAsync();
+ 客户端
   + ```            serviceCollection.AddRpcClient(configuration);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var client = serviceProvider.GetService<RpcClient>();
            await client.StartAsync();

            var userService = client.CreateProxy<IUserService>();
            for (int i = 0; i < 100; i++)
            {
                var result = userService.GetUserNameById(new TestParam { Id = 1.ToString() });
                Console.WriteLine($"{result}----{i}");
            }