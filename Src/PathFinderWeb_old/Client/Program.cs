using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PathFinderWeb.Client.Services;
using Toolbox.Services;

namespace PathFinderWeb.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddScoped<NavMenuService>();
            builder.Services.AddScoped<LinkService>();
            builder.Services.AddScoped<ClientContentService>();
            builder.Services.AddSingleton<IJson, Json>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
