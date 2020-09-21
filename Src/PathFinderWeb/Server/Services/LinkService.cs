using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Toolbox.Services;
using PathFinder.sdk.Client;
using Toolbox.Tools;
using PathFinder.sdk.Records;
using System.Threading;
using PathFinder.sdk.Models;
using PathFinderWeb.Client.Services;

namespace PathFinderWeb.Server.Services
{
    public class LinkService
    {
        private PathFinderClient _pathFinderClient;
        private ILogger<LinkService> _logger;

        public LinkService(HttpClient httpClient, IJson json, ILoggerFactory loggerFactory)
        {
            httpClient.VerifyNotNull(nameof(httpClient));
            json.VerifyNotNull(nameof(json));
            loggerFactory.VerifyNotNull(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<LinkService>();
            _logger.LogInformation($"BaseUrl: {httpClient.BaseAddress}");

            _pathFinderClient = new PathFinderClient(httpClient, json, loggerFactory);
        }

        public async Task<LinkRecord?> Get(string id, CancellationToken token = default) => await _pathFinderClient.Link.Get(id, token);

        public async Task Set(LinkRecord linkRecord, CancellationToken token = default) => await _pathFinderClient.Link.Set(linkRecord, token);

        public async Task Delete(string id, CancellationToken token = default) => await _pathFinderClient.Link.Delete(id, token);

        public async Task<BatchSet<LinkRecord>> List(int index = 0, int count = 1000, CancellationToken token = default) => await _pathFinderClient.Link.List(index, count, token);
    }
}
