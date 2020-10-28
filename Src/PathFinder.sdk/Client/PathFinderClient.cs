using Microsoft.Extensions.Logging;
using System.Net.Http;
using Toolbox.Services;
using Toolbox.Tools;

namespace PathFinder.sdk.Client
{
    public class PathFinderClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PathFinderClient> _logger;

        public PathFinderClient(HttpClient httpClient, ILogger<PathFinderClient> logger)
        {
            httpClient.VerifyNotNull(nameof(httpClient));
            logger.VerifyNotNull(nameof(logger));

            _httpClient = httpClient;
            _logger = logger;

            Link = new LinkClient(_httpClient, _logger);
            Metadata = new MetadataClient(_httpClient, _logger);
        }

        public LinkClient Link { get; }

        public MetadataClient Metadata { get; }
    }
}
