﻿using PathFinderWeb.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinderWeb.Client.Services
{
    public class ClientContentService
    {
        private readonly HttpClient _httpClient;
        private ConcurrentDictionary<string, DocItem> _cache = new ConcurrentDictionary<string, DocItem>();

        public ClientContentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetDocHtml(string id)
        {
            if (_cache.TryGetValue(id, out DocItem? docItem)) return docItem.Html;

            docItem = (await _httpClient.GetFromJsonAsync<DocItem>($"api/config/doc/{id}")).VerifyNotNull("No content");
            _cache.TryAdd(id, docItem);

            return docItem.Html;
        }
    }
}
