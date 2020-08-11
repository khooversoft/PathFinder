using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PathFinder.sdk.Actors;
using PathFinder.sdk.Host.PathServices;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Toolbox.Actor.Host;
using Toolbox.Tools;

namespace PathFinder.sdk.Services
{
    public static class PathFinderServiceExtensions
    {
        public static IActorHost AddPathServiceActors(this IActorHost actorHost, IServiceProvider serviceProvider)
        {
            actorHost.VerifyNotNull(nameof(actorHost));
            serviceProvider.VerifyNotNull(nameof(serviceProvider));

            actorHost
                .Register<ILinkRecordActor>(() => serviceProvider.GetRequiredService<ILinkRecordActor>())
                .Register<IMetadataRecordActor>(() => serviceProvider.GetRequiredService<IMetadataRecordActor>());

            return actorHost;
        }

        public static IServiceCollection AddPathServiceActorHost(this IServiceCollection services, int capacity = 10000)
        {
            services.VerifyNotNull(nameof(services));

            services.AddSingleton(x =>
            {
                ILoggerFactory loggerFactory = x.GetRequiredService<ILoggerFactory>();

                IActorHost host = new ActorHost(capacity, loggerFactory);
                host.AddPathServiceActors(x);

                return host;
            });

            return services;
        }

        public static IServiceCollection AddPathServices(this IServiceCollection services)
        {
            services.VerifyNotNull(nameof(services));

            services.AddSingleton<ILinkPathService, LinkPathService>();
            services.AddSingleton<IMetadataPathService, MetadataPathService>();

            services.AddSingleton<ILinkRecordActor, LinkRecordActor>();
            services.AddSingleton<IMetadataRecordActor, MetadataRecordActor>();

            return services;
        }
    }
}
