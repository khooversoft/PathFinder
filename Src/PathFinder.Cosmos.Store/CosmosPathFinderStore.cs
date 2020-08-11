using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Records;
using PathFinder.sdk.Store;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinder.Cosmos.Store
{
    public class CosmosPathFinderStore : IPathFinderStore
    {
        private readonly ILogger<CosmosPathFinderStore> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly ICosmosPathFinderOption _storeOption;

        public CosmosPathFinderStore(ICosmosPathFinderOption storeOption, ILoggerFactory loggerFactory)
        {
            storeOption.VerifyNotNull(nameof(storeOption)).Verify();
            loggerFactory.VerifyNotNull(nameof(loggerFactory));

            _storeOption = storeOption;
            _logger = loggerFactory.CreateLogger<CosmosPathFinderStore>();

            _cosmosClient = new CosmosClient(storeOption.GetResolvedConnectionString());

            Database = new StoreDatabase(_cosmosClient, loggerFactory.CreateLogger<StoreDatabase>());
            Container = new StoreRepository(storeOption, _cosmosClient, loggerFactory, loggerFactory.CreateLogger<StoreRepository>());
        }

        public IStoreDatabase Database { get; }

        public IStoreContainer Container { get; }

        public async Task InitializeContainers(CancellationToken token = default)
        {
            _logger.LogInformation($"{nameof(InitializeContainers)}: Initializing database with required containers");

            await Database.Create(_storeOption.DatabaseName, token);

            await Container.Create<LinkRecord>(token: token);
            await Container.Create<MetadataRecord>(token: token);
        }
    }
}
