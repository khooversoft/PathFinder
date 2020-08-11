using Microsoft.Extensions.Logging;
using PathFinder.sdk.Records;
using PathFinderCmd.Application;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Services;

namespace PathFinderCmd.Activities
{
    internal class TemplateActivity
    {
        private readonly IOption _option;
        private readonly IJson _json;
        private readonly ILogger<TemplateActivity> _logger;

        public TemplateActivity(IOption option, IJson json, ILogger<TemplateActivity> logger)
        {
            _option = option;
            _json = json;
            _logger = logger;
        }

        public Task Create()
        {
            switch (_option)
            {
                case Option option when option.Link:
                    CreateLinkTemplate(0, _option.File!);
                    break;

                case Option option when option.Metadata:
                    CreateMetadataTemplate(0, _option.File!);
                    break;
            }

            return Task.CompletedTask;
        }

        private void CreateLinkTemplate(int index, string file)
        {
            var record = new LinkRecord
            {
                Id = $"Link_{index}",
                RedirectUrl = "http://redirect",
            };

            File.WriteAllText(file, _json.SerializeFormat(record));
            _logger.LogInformation($"Create json template {file} for Link");
        }

        private void CreateMetadataTemplate(int index, string file)
        {
            var record = new MetadataRecord
            {
                Id = $"Metadata_{index}",
                Properties = new Dictionary<string, string>
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                }
            };

            File.WriteAllText(file, _json.Serialize(record));
            _logger.LogInformation($"Create json template {file} for Metadata");
        }
    }
}