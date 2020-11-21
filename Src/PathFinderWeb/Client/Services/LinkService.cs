using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinderWeb.Client.Services
{
    public class LinkService
    {
        private readonly HttpClient _httpClient;

        public LinkService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Delete(string id)
        {
            id.VerifyNotEmpty(nameof(id));

            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/link/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<LinkRecord?> Get(string id)
        {
            id.VerifyNotEmpty(nameof(id));

            return await _httpClient.GetFromJsonAsync<LinkRecord>($"api/link/{id}");
        }

        public async Task<IReadOnlyList<LinkRecord>> List(QueryParameters queryParameters)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/link/list", queryParameters);
            response.EnsureSuccessStatusCode();

            BatchSet<LinkRecord> result = (await response.Content.ReadFromJsonAsync<BatchSet<LinkRecord>>()).VerifyNotNull("No response");
            return result.Records;
        }

        public async Task Set(LinkRecord linkRecord)
        {
            linkRecord.VerifyNotNull(nameof(linkRecord));

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/link", linkRecord);
            response.EnsureSuccessStatusCode();
        }
    }
}