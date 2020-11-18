using Microsoft.Extensions.Logging;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinder.sdk.Store;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;
using Toolbox.Tools;

namespace PathFinder.sdk.Actors
{
    public class LinkRecordActor : ActorBase, ILinkRecordActor
    {
        private readonly IRecordContainer<LinkRecord> _container;
        private readonly ILogger<LinkRecordActor> _logger;
        private CacheObject<LinkRecord> _recordCache = new CacheObject<LinkRecord>(TimeSpan.FromMinutes(10));

        public LinkRecordActor(IRecordContainer<LinkRecord> linkContainer, ILogger<LinkRecordActor> logger)
        {
            _container = linkContainer;
            _logger = logger;
        }

        public async Task<LinkRecord?> Get(CancellationToken token)
        {
            if (_recordCache.TryGetValue(out LinkRecord value)) return value;

            Record<LinkRecord>? result = await _container.Get(base.ActorKey.Value, token: token);
            if (result == null) return null;

            _recordCache.Set(result!.Value);
            return result!.Value;
        }

        public async Task Set(LinkRecord record, CancellationToken token)
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

            bool state = await _container.Delete(base.ActorKey.Value, token: token);

            await Deactivate();
            return state;
        }
    }
}
