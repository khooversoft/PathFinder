using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PathFinder.Cosmos.Store;
using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Actors;
using PathFinder.sdk.Host.PathServices;
using PathFinder.sdk.Records;
using PathFinder.sdk.Services;
using PathFinder.sdk.Store;
using PathFinderApi.Application;
using Toolbox.Services;

namespace PathFinderApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IJson, Json>();

            services.AddSingleton<IPathFinderStore, CosmosPathFinderStore>();

            services.AddPathServiceActorHost();
            services.AddPathServices();
            //services.AddSingleton<ILinkPathService, LinkPathService>();
            //services.AddSingleton<IMetadataPathService, MetadataPathService>();

            //services.AddSingleton<ILinkRecordActor, LinkRecordActor>();
            //services.AddSingleton<IMetadataRecordActor, MetadataRecordActor>();

            services.AddSingleton<IRecordContainer<LinkRecord>>(services =>
            {
                IPathFinderStore store = services.GetRequiredService<IPathFinderStore>();
                return store.Container.Get<LinkRecord>();
            });

            services.AddSingleton<IRecordContainer<MetadataRecord>>(services =>
            {
                IPathFinderStore store = services.GetRequiredService<IPathFinderStore>();
                return store.Container.Get<MetadataRecord>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
