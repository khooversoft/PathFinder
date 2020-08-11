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

        public MetadataClient(RestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<MetadataRecord?> Get(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));

            return await _restClient
                .AddPath($"api/metadata/{id}")
                .SetEnsureSuccessStatusCode()
                .SetValidHttpStatusCodes(HttpStatusCode.NotFound)
                .GetAsync(token)
                .GetContentAsync<MetadataRecord?>();
        }

        public async Task Set(MetadataRecord linkRecord, CancellationToken token = default)
        {
            linkRecord
                .VerifyNotNull(nameof(linkRecord))
                .Prepare();

            await _restClient
                .AddPath($"api/metadata")
                .SetContent(linkRecord)
                .SetEnsureSuccessStatusCode()
                .PostAsync(token);
        }

        public async Task Delete(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));

            await _restClient
                .AddPath($"api/metadata/{id}")
                .SetEnsureSuccessStatusCode()
                .DeleteAsync(token);
        }

        public async Task<BatchSet<MetadataRecord>> List(int index = 0, int count = 1000, CancellationToken token = default)
        {
            return await _restClient
                .AddPath($"api/metadata/list/{index}/{count}")
                .SetEnsureSuccessStatusCode()
                .GetAsync(token)
                .GetContentAsync<BatchSet<MetadataRecord>>();
        }

    }
}
