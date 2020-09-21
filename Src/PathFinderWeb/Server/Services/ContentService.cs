using Markdig;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace PathFinderWeb.Server.Services
{
    public class ContentService
    {
        private ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

        public ContentService() { }

        public string GetDocHtml(string id)
        {
            if (_cache.TryGetValue(id, out string? html)) return html;

            return AddResource(id);
        }

        private string AddResource(string id)
        {
            string path = $"PathFinderWeb.Server.Application.Data.{id}";

            using Stream resource = Assembly.GetAssembly(typeof(ContentService))
                ?.GetManifestResourceStream(path)
                .VerifyNotNull($"Cannot find doc {id}")!;

            using StreamReader reader = new StreamReader(resource);
            string resourceHtml = reader.ReadToEnd();

            string mdSource = Transform(resourceHtml);
            _cache.TryAdd(id, mdSource);

            return mdSource;
        }

        private string Transform(string mdSource)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            var result = Markdown.ToHtml(mdSource, pipeline);

            return result;
        }
    }
}
