using Microsoft.Extensions.Logging;
using PathFinder.sdk.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinder.sdk.Client
{
    public class BatchSetCursor<T>
    {
        private static readonly BatchSet<T> _noResult = new BatchSet<T>();
        private readonly HttpClient _httpClient;
        private readonly QueryParameters _queryParameters;
        private readonly ILogger _logger;
        private Func<CancellationToken, Task<BatchSet<T>>> _getFunc;

        public BatchSetCursor(HttpClient httpClient, QueryParameters queryParameters, ILogger logger)
        {
            _httpClient = httpClient;
            _queryParameters = queryParameters;
            _logger = logger;

            _getFunc = Start;
        }

        public BatchSet<T>? Current { get; private set; }

        public async Task<BatchSet<T>> ReadNext(CancellationToken token = default) => await _getFunc(token);

        private async Task<BatchSet<T>> Start(CancellationToken token)
        {
            string query = _queryParameters.ToQuery();
            _logger.LogTrace($"{nameof(Start)}: Query={query}");

            Current = await _httpClient.GetFromJsonAsync<BatchSet<T>>($"api/link/list?{query}", token);
            _getFunc = Continue;

            return Current;
        }

        private async Task<BatchSet<T>> Continue(CancellationToken token)
        {
            _logger.LogTrace($"{nameof(Continue)}: continuationUrl={Current!.ContinuationUrl}");

            Current = await _httpClient.GetFromJsonAsync<BatchSet<T>>(Current!.ContinuationUrl, token);
            _getFunc = Continue;

            return Current;
        }
    }
}
