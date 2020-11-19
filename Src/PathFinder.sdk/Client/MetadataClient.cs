using Microsoft.Extensions.Logging;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinder.sdk.Client
{
    public class MetadataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public MetadataClient(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<MetadataRecord?> Get(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));
            _logger.LogTrace($"{nameof(Get)}: Id={id}");

            try
            {
                return await _httpClient.GetFromJsonAsync<MetadataRecord>($"api/metadata/{id}", token);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{nameof(Get)}: id={id} failed");
                return null;
            }
        }

        public async Task Set(MetadataRecord metadataRecord, CancellationToken token = default)
        {
            _logger.LogTrace($"{nameof(Set)}: Id={metadataRecord.Id}");

            metadataRecord
                .VerifyNotNull(nameof(metadataRecord))
                .Prepare();

            await _httpClient.PostAsJsonAsync("api/metadata", metadataRecord, token);
        }

        public async Task<bool> Delete(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));
            _logger.LogTrace($"{nameof(Delete)}: Id={id}");

            HttpResponseMessage? response = await _httpClient.DeleteAsync($"api/metadata/{id}");
            return response.IsSuccessStatusCode;
        }

        public BatchSetCursor<MetadataRecord> List(QueryParameters queryParameters) => new BatchSetCursor<MetadataRecord>(_httpClient, queryParameters, _logger);
    }
}
