using Microsoft.Extensions.Logging;
using PathFinder.sdk.Client;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinderWeb.Server.Services
{
    public class MetadataService
    {
        private PathFinderClient _pathFinderClient;

        public MetadataService(HttpClient httpClient, ILoggerFactory loggerFactory)
        {
            httpClient.VerifyNotNull(nameof(httpClient));
            loggerFactory.VerifyNotNull(nameof(loggerFactory));

            _pathFinderClient = new PathFinderClient(httpClient, loggerFactory.CreateLogger<PathFinderClient>());
        }

        public async Task<MetadataRecord?> Get(string id, CancellationToken token = default) => await _pathFinderClient.Metadata.Get(id, token);

        public async Task Set(MetadataRecord metadataREcord, CancellationToken token = default) => await _pathFinderClient.Metadata.Set(metadataREcord, token);

        public async Task Delete(string id, CancellationToken token = default) => await _pathFinderClient.Link.Delete(id, token);

        public async Task<BatchSet<MetadataRecord>> List(QueryParameters queryParameters, CancellationToken token = default) => await _pathFinderClient.Metadata.List(queryParameters).ReadNext(token);
    }
}