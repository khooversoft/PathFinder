using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using PathFinder.Cosmos.Store;
using PathFinder.sdk.Records;
using PathFinder.sdk.Services;
using PathFinder.sdk.Store;
using System;
using Toolbox.Logging;
using Toolbox.Services;

namespace PathFinder.Server
{
    public class Startup
    {
        private const string _policyName = "defaultPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IPathFinderStore, CosmosPathFinderStore>();
            services.AddSingleton<ITelemetryMemory, TelemetryMemory>();

            // Register path services actor host (bind DI to actor)
            services.AddPathServiceActorHost();

            // Register path services (link & metadata)
            services.AddPathServices();

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

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Title = "Path Finder - Links and Metadata";
                    document.Info.Description = "API Server for Path Finder services";
                    document.Schemes = new[] { OpenApiSchema.Http | OpenApiSchema.Https };
                };
            });

            services.AddCors(x => x.AddPolicy(_policyName, builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetPreflightMaxAge(TimeSpan.FromHours(1));
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, ITelemetryMemory telemetryMemory)
        {
            loggerFactory.AddProvider(new TelemetryMemoryLoggerProvider(telemetryMemory));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(_policyName);
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}