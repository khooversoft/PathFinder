using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using PathFinder.sdk.Application;
using PathFinderWeb.Server.Application;

namespace PathFinderWeb.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IOption option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            CreateHostBuilder(args, option)
                .Build()
                .Run();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args, IOption option) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(config =>
                {
                    config.AddSingleton(option);
                })
                .ConfigureLogging(builder =>
                {
                    if (option.RunEnvironment == RunEnvironment.Dev)
                    {
                        builder.AddDebug();
                        builder.AddFilter<DebugLoggerProvider>(x => true);
                        builder.AddFilter(x => true);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
