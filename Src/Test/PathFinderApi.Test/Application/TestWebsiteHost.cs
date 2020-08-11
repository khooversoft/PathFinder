using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using PathFinderApi.Application;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Toolbox.Services;
using Toolbox.Tools;
using PathFinder.sdk.Client;
using PathFinder.Cosmos.Store.Application;

namespace PathFinderApi.Test.Application
{
    internal class TestWebsiteHost
    {
        protected IHost? _host;
        protected HttpClient? _client;

        public TestWebsiteHost() { }

        public HttpClient Client => _client ?? throw new ArgumentNullException(nameof(Client));

        public T Resolve<T>() where T : class => _host?.Services.GetService<T>() ?? throw new InvalidOperationException($"Cannot find service {typeof(T).Name}");

        public PathFinderClient PathFinderClient => new PathFinderClient(Client, Resolve<IJson>(), Resolve<ILoggerFactory>());

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
            string packageFile = FileTools.WriteResourceToTempFile("PathFinderApi.Test", nameof(TestWebsiteHost), typeof(TestWebsiteHost), "PathFinderApi.Test.Application.Config.json");
            string[] args = new[]
            {
                $"ConfigFile={packageFile}",
            };

            return new OptionBuilder()
                .SetArgs(args)
                .Build();
        }
    }
}
