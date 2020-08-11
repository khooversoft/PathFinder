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

        public LinkClient(RestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<LinkRecord?> Get(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));

            return await _restClient
                .AddPath($"api/link/{id}")
                .SetEnsureSuccessStatusCode()
                .SetValidHttpStatusCodes(HttpStatusCode.NotFound)
                .GetAsync(token)
                .GetContentAsync<LinkRecord?>();
        }

        public async Task Set(LinkRecord linkRecord, CancellationToken token = default)
        {
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

            await _restClient
                .AddPath($"api/link/{id}")
                .SetEnsureSuccessStatusCode()
                .DeleteAsync(token);
        }

        public async Task<BatchSet<LinkRecord>> List(int index = 0, int count = 1000, CancellationToken token = default)
        {
            return await _restClient
                .AddPath($"api/link/list/{index}/{count}")
                .SetEnsureSuccessStatusCode()
                .GetAsync(token)
                .GetContentAsync<BatchSet<LinkRecord>>();
        }
    }
}
