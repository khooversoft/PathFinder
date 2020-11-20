using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinderWeb.Client.Services
{
    public class MetadataService
    {
        private readonly HttpClient _httpClient;

        public MetadataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Delete(string id)
        {
            id.VerifyNotEmpty(nameof(id));

            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/metadata/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<MetadataRecord> Get(string id)
        {
            id.VerifyNotEmpty(nameof(id));

            return await _httpClient.GetFromJsonAsync<MetadataRecord>($"api/metadata/{id}");
        }

        public async Task<IReadOnlyList<MetadataRecord>> List(QueryParameters queryParameters)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/metadata/list", queryParameters);
            response.EnsureSuccessStatusCode();

            BatchSet<MetadataRecord> result = await response.Content.ReadFromJsonAsync<BatchSet<MetadataRecord>>();
            return result.Records;
        }

        public async Task Set(MetadataRecord metadataRecord)
        {
            metadataRecord.VerifyNotNull(nameof(metadataRecord));

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/metadata", metadataRecord);
            response.EnsureSuccessStatusCode();
        }
    }
}