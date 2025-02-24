using System;
using Adnc.Infra.Consul;
using Adnc.Infra.Consul.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Adnc.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                            .ConfigureAppConfiguration((context, cb) =>
                            {
                               //生产环境从consul配置中心读取配置
                                var env = context.HostingEnvironment;
                                if (env.IsProduction() || env.IsStaging())
                                {
                                    var configuration = cb.Build();
                                    var consulOption = configuration.GetSection("Consul").Get<ConsulConfig>();
                                    cb.AddConsulConfiguration(new[] { consulOption.ConsulUrl }, consulOption.ConsulKeyPath);
                                }
                            })
                            .ConfigureAppConfiguration((hostingContext, config) =>
                            {
                                var env = hostingContext.HostingEnvironment;
                                config.AddJsonFile($"{AppContext.BaseDirectory}/Config/ocelot.{env.EnvironmentName}.json", false, true);
                            })
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseStartup<Startup>();
                            });
    }
}
