using Microsoft.Extensions.Logging;
using PathFinder.sdk.Records;
using PathFinder.sdk.Services.RecordAbstract;
using PathFinder.sdk.Store;
using PathFinderCmd.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace PathFinderCmd.Activities
{
    internal class ImportActivity
    {
        private readonly IOption _option;
        private readonly IRecordContainer<LinkRecord> _linkContainer;
        private readonly IRecordContainer<MetadataRecord> _metadataContainer;
        private readonly IJson _json;
        private readonly ILogger<ImportActivity> _logger;

        public ImportActivity(IOption option, IRecordContainer<LinkRecord> linkContainer, IRecordContainer<MetadataRecord> metadataContainer, IJson json, ILogger<ImportActivity> logger)
        {
            _option = option;
            _linkContainer = linkContainer;
            _metadataContainer = metadataContainer;
            _json = json;
            _logger = logger;
        }

        public async Task Import(CancellationToken token)
        {
            IReadOnlyList<string> files = GetFiles(_option.File!);

            foreach (var file in files)
            {
                _logger.LogInformation($"Importing configuration {file}");

                string json = File.ReadAllText(file);
                switch (_json.Deserialize<RecordBase>(json))
                {
                    case RecordBase v:
                        await WriteRecord(v.RecordType, json, token);
                        break;

                    default:
                        throw new ArgumentException($"Bad format");
                }
            }
        }

        private async Task WriteRecord(string recordType, string json, CancellationToken token)
        {
            switch (recordType)
            {
                case nameof(LinkRecord):
                    LinkRecord linkRecord = _json.Deserialize<LinkRecord>(json);
                    await _linkContainer.Set(linkRecord, token);
                    break;

                case nameof(MetadataRecord):
                    MetadataRecord metadataRecord = _json.Deserialize<MetadataRecord>(json);
                    await _metadataContainer.Set(metadataRecord, token);
                    break;

                default:
                    throw new ArgumentException($"Unknown record type for importing, recordType={recordType}");
            }
        }

        private IReadOnlyList<string> GetFiles(string file)
        {
            if (Directory.Exists(file))
            {
                return Directory.GetFiles(file, "*.json", SearchOption.TopDirectoryOnly);
            }

            bool recursiveFolderSearch = file.EndsWith("\\**");
            bool folderSearch = file.EndsWith("\\*");
            string folder = Path.GetDirectoryName(file)!;
            string search = recursiveFolderSearch || folderSearch ? "*.json" : Path.GetFileName(file);

            _logger.LogInformation($"{nameof(GetFiles)}: Searching folder={folder}, search={search}");
            string[] files = Directory.GetFiles(folder, search, recursiveFolderSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            files.VerifyAssert(x => x.Length > 0, $"File(s) {file} does not exist");
            return files;
        }
    }
}
