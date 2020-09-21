using Microsoft.Extensions.Logging;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;
using Toolbox.Tools.Rest;

namespace PathFinder.sdk.Client
{
    public class LinkClient
    {
        private readonly RestClient _restClient;
        private readonly ILogger _logger;

        public LinkClient(RestClient restClient, ILogger logger)
        {
            _restClient = restClient;
            _logger = logger;
        }

        public async Task<LinkRecord?> Get(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));
            _logger.LogTrace($"{nameof(Get)}: Id={id}");

            return await _restClient
                .AddPath($"api/link/{id}")
                .SetEnsureSuccessStatusCode()
                .SetValidHttpStatusCodes(HttpStatusCode.NotFound)
                .GetAsync(token)
                .GetContentAsync<LinkRecord?>();
        }

        public async Task Set(LinkRecord linkRecord, CancellationToken token = default)
        {
            _logger.LogTrace($"{nameof(Set)}: Id={linkRecord.Id}");

            linkRecord
                .VerifyNotNull(nameof(linkRecord))
                .Prepare();

            await _restClient
                .AddPath($"api/link")
                .SetContent(linkRecord)
                .SetEnsureSuccessStatusCode()
                .PostAsync(token);
        }

        public async Task Delete(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));
            _logger.LogTrace($"{nameof(Delete)}: Id={id}");

            await _restClient
                .AddPath($"api/link/{id}")
                .SetEnsureSuccessStatusCode()
                .DeleteAsync(token);
        }

        public async Task<BatchSet<LinkRecord>> List(int index = 0, int count = 1000, CancellationToken token = default)
        {
            _logger.LogTrace($"{nameof(List)}: Index={index}");

            return await _restClient
                .AddPath($"api/link/list/{index}/{count}")
                .SetEnsureSuccessStatusCode()
                .GetAsync(token)
                .GetContentAsync<BatchSet<LinkRecord>>();
        }
    }
}
