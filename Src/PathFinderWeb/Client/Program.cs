using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PathFinderWeb.Client.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
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

            builder.RootComponents.Add<App>("app");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}