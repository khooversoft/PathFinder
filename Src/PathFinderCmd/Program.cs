using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PathFinder.Cosmos.Store;
using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Records;
using PathFinder.sdk.Store;
using PathFinderCmd.Activities;
using PathFinderCmd.Application;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Logging;
using Toolbox.Services;

[assembly: InternalsVisibleTo("PathFinderCmd.Test")]

namespace PathFinderCmd
{
    internal class Program
    {
        private const int _ok = 0;
        private const int _error = 1;
        private readonly string _programTitle = $"Watcher CLI - Version {Assembly.GetExecutingAssembly().GetName().Version}";

        internal static async Task<int> Main(string[] args)
        {
            try
            {
                return await new Program().Run(args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                DisplayStartDetails(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhanded exception: " + ex.ToString());
                DisplayStartDetails(args);
            }

            return _error;
        }

        private static void DisplayStartDetails(string[] args) => Console.WriteLine($"Arguments: {string.Join(", ", args)}");

        private async Task<int> Run(string[] args)
        {
            Console.WriteLine(_programTitle);
            Console.WriteLine();

            IOption option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            if (option.Help)
            {
                option.GetHelp()
                    .Append(string.Empty)
                    .ForEach(x => Console.WriteLine(x));

                return _ok;
            }

            option.DumpConfigurations();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            using (ServiceProvider serviceProvider = CreateContainer(option))
            {
                IServiceProvider container = serviceProvider;

                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                {
                    e.Cancel = true;
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Canceling...");
                };

                var activities = new Func<Task>[]
                {
                    () => option.Initialize || option.Link || option.Metadata || option.Import
                        ? InitializeRepository(option, container, cancellationTokenSource.Token)
                        : Task.CompletedTask,

                    () => option.Link && option.Get ? container.GetRequiredService<LinkActivity>().Get(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Link && option.List ? container.GetRequiredService<LinkActivity>().List(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Link && option.Delete ? container.GetRequiredService<LinkActivity>().Delete(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Link && option.Clear ? container.GetRequiredService<LinkActivity>().Clear(cancellationTokenSource.Token) : Task.CompletedTask,

                    () => option.Metadata && option.Get ? container.GetRequiredService<MetadataActivity>().Get(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Metadata && option.List ? container.GetRequiredService<MetadataActivity>().List(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Metadata && option.Delete ? container.GetRequiredService<MetadataActivity>().Delete(cancellationTokenSource.Token) : Task.CompletedTask,
                    () => option.Metadata && option.Clear ? container.GetRequiredService<MetadataActivity>().Clear(cancellationTokenSource.Token) : Task.CompletedTask,

                    () => option.Template ? container.GetRequiredService<TemplateActivity>().Create() : Task.CompletedTask,
                    () => option.Import ? container.GetRequiredService<ImportActivity>().Import(cancellationTokenSource.Token) : Task.CompletedTask,
                };

                await activities
                    .ForEachAsync(async x => await x());
            }

            Console.WriteLine();
            Console.WriteLine("Completed");
            return _ok;
        }

        private ServiceProvider CreateContainer(IOption option)
        {
            var container = new ServiceCollection();

            container.AddHttpClient();

            container.AddLogging(config =>
            {
                config
                    .AddConsole()
                    .AddDebug();

                if (!option.LogFolder.IsEmpty()) config.AddFileLogger(option.LogFolder!, "PathFinder");
            });

            container.AddSingleton(option);
            container.AddSingleton<IJson, Json>();
            container.AddSingleton<TemplateActivity>();

            if (option.Store != null)
            {
                container.AddSingleton<ICosmosPathFinderOption>(option.Store);
                container.AddSingleton<IPathFinderStore, CosmosPathFinderStore>();

                container.AddSingleton<IRecordContainer<LinkRecord>>(services =>
                {
                    IPathFinderStore store = services.GetRequiredService<IPathFinderStore>();
                    return store.Container.Get<LinkRecord>();
                });

                container.AddSingleton<IRecordContainer<MetadataRecord>>(services =>
                {
                    IPathFinderStore store = services.GetRequiredService<IPathFinderStore>();
                    return store.Container.Get<MetadataRecord>();
                });

                container.AddSingleton<LinkActivity>();
                container.AddSingleton<MetadataActivity>();
                container.AddSingleton<ImportActivity>();
            }

            return container.BuildServiceProvider();
        }

        private async Task InitializeRepository(IOption option, IServiceProvider container, CancellationToken token)
        {
            if (option.Store == null) return;

            await container.GetRequiredService<IPathFinderStore>()
                .InitializeContainers(token);
        }
    }
}
