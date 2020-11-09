using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Store;
using PathFinder.Server.Application;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("PathFinder.Server.Test")]

namespace PathFinder.Server
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            IOption option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            IHost host = CreateHostBuilder(args, option)
                .Build();

            await InitializeDatabase(host, option);

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IOption option) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IOption>(option);
                    services.AddSingleton<ICosmosPathFinderOption>(option.Store);
                })
                .ConfigureLogging(config =>
                {
                    config
                        .AddConsole()
                        .AddDebug()
                        .AddFilter(x => true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    if (option.ApplicationUrl != null) webBuilder.UseUrls(option.ApplicationUrl);
                });

        private static async Task InitializeDatabase(IHost host, IOption option)
        {
            if (!option.InitializeDatabase) return;

            CancellationToken token = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;
            using var scope = host.Services.CreateScope();

            await scope.ServiceProvider.GetRequiredService<IPathFinderStore>()
                .InitializeContainers(token);
        }
    }
}

