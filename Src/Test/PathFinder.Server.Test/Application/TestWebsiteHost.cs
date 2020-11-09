using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Client;
using PathFinder.sdk.Store;
using PathFinder.Server.Application;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinder.Server.Test.Application
{
    internal class TestWebsiteHost
    {
        protected IHost? _host;
        protected HttpClient? _client;

        public TestWebsiteHost() { }

        public HttpClient Client => _client ?? throw new ArgumentNullException(nameof(Client));

        public T Resolve<T>() where T : class => _host?.Services.GetService<T>() ?? throw new InvalidOperationException($"Cannot find service {typeof(T).Name}");

        public PathFinderClient PathFinderClient => new PathFinderClient(Client, Resolve<ILoggerFactory>().CreateLogger<PathFinderClient>());

        public TestWebsiteHost StartApiServer()
        {
            IOption option = GetOption();

            var host = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .ConfigureLogging(builder => builder.AddDebug())
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(option)
                        .AddSingleton<ICosmosPathFinderOption>(option.Store);
                });

            _host = host.Start();
            _client = _host.GetTestServer().CreateClient();

            InitializeDatabase(_host, option).Wait();
            return this;
        }

        public void Shutdown()
        {
            Interlocked.Exchange(ref _client, null)?.Dispose();

            var host = Interlocked.Exchange(ref _host, null);
            if (host != null)
            {
                try
                {
                    host.StopAsync(TimeSpan.FromMinutes(10)).Wait();
                }
                catch { }
                finally
                {
                    host.Dispose();
                }
            }
        }

        private IOption GetOption()
        {
            string packageFile = FileTools.WriteResourceToTempFile("PathFinder.Server.Test", nameof(TestWebsiteHost), typeof(TestWebsiteHost), "PathFinder.Server.Test.Application.Config.json");
            string[] args = new[]
            {
                $"ConfigFile={packageFile}",
            };

            return new OptionBuilder()
                .SetArgs(args)
                .Build();
        }

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
