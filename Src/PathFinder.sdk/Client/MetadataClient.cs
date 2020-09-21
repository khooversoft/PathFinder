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
    public class MetadataClient
    {
        private readonly RestClient _restClient;
        private readonly ILogger _logger;

        public MetadataClient(RestClient restClient, ILogger logger)
        {
            _restClient = restClient;
            _logger = logger;
        }

        public async Task<MetadataRecord?> Get(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));
            _logger.LogTrace($"{nameof(Get)}: Id={id}");

            return await _restClient
                .AddPath($"api/metadata/{id}")
                .SetEnsureSuccessStatusCode()
                .SetValidHttpStatusCodes(HttpStatusCode.NotFound)
                .GetAsync(token)
                .GetContentAsync<MetadataRecord?>();
        }

        public async Task Set(MetadataRecord metadataRecord, CancellationToken token = default)
        {
            _logger.LogTrace($"{nameof(Set)}: Id={metadataRecord.Id}");

            metadataRecord
                .VerifyNotNull(nameof(metadataRecord))
                .Prepare();

            await _restClient
                .AddPath($"api/metadata")
                .SetContent(metadataRecord)
                .SetEnsureSuccessStatusCode()
                .PostAsync(token);
        }

        public async Task Delete(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));
            _logger.LogTrace($"{nameof(Delete)}: Id={id}");

            await _restClient
                .AddPath($"api/metadata/{id}")
                .SetEnsureSuccessStatusCode()
                .DeleteAsync(token);
        }

        public async Task<BatchSet<MetadataRecord>> List(int index = 0, int count = 1000, CancellationToken token = default)
        {
            _logger.LogTrace($"{nameof(List)}: Index={index}");

            return await _restClient
                .AddPath($"api/metadata/list/{index}/{count}")
                .SetEnsureSuccessStatusCode()
                .GetAsync(token)
                .GetContentAsync<BatchSet<MetadataRecord>>();
        }

    }
}
