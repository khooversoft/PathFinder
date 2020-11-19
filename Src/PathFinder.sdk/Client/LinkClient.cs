using Microsoft.Extensions.Logging;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinder.sdk.Client
{
    public class LinkClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public LinkClient(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<LinkRecord?> Get(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));
            _logger.LogTrace($"{nameof(Get)}: Id={id}");

            try
            {
                return await _httpClient.GetFromJsonAsync<LinkRecord?>($"api/link/{id}", token);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{nameof(Get)}: id={id} failed");
                return null;
            }
        }

        public async Task Set(LinkRecord linkRecord, CancellationToken token = default)
        {
            _logger.LogTrace($"{nameof(Set)}: Id={linkRecord.Id}");

            linkRecord
                .VerifyNotNull(nameof(linkRecord))
                .Prepare();

            await _httpClient.PostAsJsonAsync("api/link", linkRecord, token);
        }

        public async Task<bool> Delete(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));
            _logger.LogTrace($"{nameof(Delete)}: Id={id}");

            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/link/{id}", token);
            return response.IsSuccessStatusCode;
        }

        public BatchSetCursor<LinkRecord> List(QueryParameters queryParameters) => new BatchSetCursor<LinkRecord>(_httpClient, queryParameters, _logger);
    }
}