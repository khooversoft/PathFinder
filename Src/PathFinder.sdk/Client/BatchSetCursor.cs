using Microsoft.Extensions.Logging;
using PathFinder.sdk.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

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
            _logger.LogTrace($"{nameof(Start)}: Query={_queryParameters}");

            Current = await Post(_queryParameters);
            _getFunc = Continue;

            return Current;
        }

        private async Task<BatchSet<T>> Continue(CancellationToken token)
        {
            _logger.LogTrace($"{nameof(Continue)}: Query={_queryParameters}");

            QueryParameters queryParameters = _queryParameters.WithIndex(Current!.NextIndex);

            Current = await Post(queryParameters);

            _getFunc = Continue;
            return Current;
        }

        private async Task<BatchSet<T>> Post(QueryParameters queryParameters)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/link/list", queryParameters);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BatchSet<T>>();
        }
    }
}