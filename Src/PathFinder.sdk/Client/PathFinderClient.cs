using Microsoft.Extensions.Logging;
using System.Net.Http;
using Toolbox.Services;
using Toolbox.Tools;
using Toolbox.Tools.Rest;

namespace PathFinder.sdk.Client
{
    public class PathFinderClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PathFinderClient> _logger;
        private readonly RestClient _restClient;

        public PathFinderClient(HttpClient httpClient, IJson json, ILoggerFactory loggerFactory)
        {
            httpClient.VerifyNotNull(nameof(httpClient));
            json.VerifyNotNull(nameof(json));
            loggerFactory.VerifyNotNull(nameof(loggerFactory));

            _httpClient = httpClient;
            _logger = loggerFactory.CreateLogger<PathFinderClient>();

            _restClient = new RestClient(httpClient, loggerFactory.CreateLogger<RestClient>(), json);
            Link = new LinkClient(_restClient);
            Metadata = new MetadataClient(_restClient);
        }

        public LinkClient Link { get; }

        public MetadataClient Metadata { get; }
    }
}
