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
    public class LinkService
    {
        private PathFinderClient _pathFinderClient;

        public LinkService(HttpClient httpClient, ILoggerFactory loggerFactory)
        {
            httpClient.VerifyNotNull(nameof(httpClient));
            loggerFactory.VerifyNotNull(nameof(loggerFactory));

            _pathFinderClient = new PathFinderClient(httpClient, loggerFactory.CreateLogger<PathFinderClient>());
        }

        public async Task<LinkRecord?> Get(string id, CancellationToken token = default) => await _pathFinderClient.Link.Get(id, token);

        public async Task Set(LinkRecord linkRecord, CancellationToken token = default) => await _pathFinderClient.Link.Set(linkRecord, token);

        public async Task Delete(string id, CancellationToken token = default) => await _pathFinderClient.Link.Delete(id, token);

        public async Task<BatchSet<LinkRecord>> List(int index = 0, int count = 1000, CancellationToken token = default) => await _pathFinderClient.Link.List(index, count, token);
    }
}