using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using PathFinder.sdk.Store;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinder.Cosmos.Store
{
    public class StoreDatabase : IStoreDatabase
    {
        private readonly CosmosClient _cosmosClient;
        private readonly ILogger<StoreDatabase> _logger;

        public StoreDatabase(CosmosClient cosmosClient, ILogger<StoreDatabase> logger)
        {
            _cosmosClient = cosmosClient;
            _logger = logger;
        }

        public async Task Create(string databaseName, CancellationToken token = default)
        {
            databaseName.VerifyNotEmpty(nameof(databaseName));

            _logger.LogTrace($"{nameof(Create)}: Create databaseName={databaseName}");
            Database database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName, cancellationToken: token);
        }

        public async Task<bool> Delete(string databaseName, CancellationToken token = default)
        {
            databaseName.VerifyNotEmpty(nameof(databaseName));
            Database database = _cosmosClient.GetDatabase(databaseName);

            _logger.LogTrace($"{nameof(Delete)}: DatabaseName={databaseName}");

            try
            {
                DatabaseResponse response = await database.DeleteAsync(cancellationToken: token);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            return true;
        }
    }
}
