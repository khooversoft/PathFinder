using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using PathFinder.sdk.Application;
using PathFinderWeb.Server.Application;

namespace PathFinderWeb.Server
{
    internal static class Program
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

        private static IHostBuilder CreateHostBuilder(string[] args, IOption option) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(config =>
                {
                    config.AddSingleton(option);
                })
                .ConfigureLogging(builder =>
                {
                    if (option.RunEnvironment == RunEnvironment.Local)
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