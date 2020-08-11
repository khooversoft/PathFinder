using Microsoft.Extensions.Logging;
using PathFinder.sdk.Records;
using PathFinder.sdk.Services.RecordAbstract;
using PathFinder.sdk.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;
using Toolbox.Tools;

namespace PathFinder.sdk.Actors
{
    public class MetadataRecordActor : ActorBase, IMetadataRecordActor
    {
        private readonly IRecordContainer<MetadataRecord> _container;
        private readonly ILogger<MetadataRecordActor> _logger;
        private readonly CacheObject<MetadataRecord> _recordCache = new CacheObject<MetadataRecord>(TimeSpan.FromMinutes(10));

        public MetadataRecordActor(IRecordContainer<MetadataRecord> linkContainer, ILogger<MetadataRecordActor> logger)
        {
            _container = linkContainer;
            _logger = logger;
        }

        public async Task<MetadataRecord?> Get(CancellationToken token)
        {
            if (_recordCache.TryGetValue(out MetadataRecord value)) return value;


            Record<MetadataRecord>? result = await _container.Get(base.ActorKey.Value, token: token);
            if (result == null) return null;

            _recordCache.Set(result!.Value);
            return result!.Value;
        }

        public async Task Set(MetadataRecord record, CancellationToken token)
        {
            record
                .VerifyNotNull(nameof(record))
                .VerifyAssert(x => x.Id == base.ActorKey.Value, "Id mismatch");

            _logger.LogTrace($"{nameof(Set)}: Writing {record}");
            await _container.Set(record, token);

            _recordCache.Set(record);
        }

        public async Task<bool> Delete(CancellationToken token)
        {
            _recordCache.Clear();
            return await _container.Delete(base.ActorKey.Value, token: token);
        }
    }
}
