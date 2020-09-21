﻿using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
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

        public async Task<LinkRecord> Get(string id)
        {
            id.VerifyNotEmpty(nameof(id));

            return await _httpClient.GetFromJsonAsync<LinkRecord>($"api/link/{id}");
        }

        public async Task<IReadOnlyList<LinkRecord>> List()
        {
            BatchSet<LinkRecord> result = await _httpClient.GetFromJsonAsync<BatchSet<LinkRecord>>("api/link/list/0/1000");
            return result.Records;
        }

        public async Task Set(LinkRecord linkRecord)
        {
            linkRecord.VerifyNotNull(nameof(linkRecord));

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/link", linkRecord);
            response.EnsureSuccessStatusCode();
        }

        public async Task Delete(string id)
        {
            id.VerifyNotEmpty(nameof(id));

            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/link/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}