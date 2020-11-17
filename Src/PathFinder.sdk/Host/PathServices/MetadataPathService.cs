using Microsoft.Extensions.DependencyInjection;
using PathFinder.sdk.Actors;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinder.sdk.Store;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;
using Toolbox.Actor.Host;
using Toolbox.Tools;

namespace PathFinder.sdk.Host.PathServices
{
    public class MetadataPathService : IMetadataPathService
    {
        private readonly IRecordContainer<MetadataRecord> _metadataContainer;
        public IActorHost? _actorHost;

        public MetadataPathService(IActorHost actorHost, IRecordContainer<MetadataRecord> metadataContainer)
        {
            _actorHost = actorHost;
            _metadataContainer = metadataContainer;
        }

        public string Name { get; } = nameof(IMetadataPathService).ToLowerInvariant();

        public async Task<MetadataRecord?> Get(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));

            IMetadataRecordActor actor = _actorHost!.GetActor<IMetadataRecordActor>((ActorKey)id);
            return await actor.Get(token);
        }

        public async Task Set(MetadataRecord record, CancellationToken token = default)
        {
            record.VerifyNotNull(nameof(record));

            IMetadataRecordActor actor = _actorHost!.GetActor<IMetadataRecordActor>((ActorKey)record.Id);
            await actor.Set(record, token);
        }

        public async Task Delete(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));

            IMetadataRecordActor actor = _actorHost!.GetActor<IMetadataRecordActor>((ActorKey)id);
            await actor.Delete(token);
        }

        public async Task<IReadOnlyList<MetadataRecord>> List(QueryParameters queryParameters, CancellationToken token = default) => await _metadataContainer.Search.List(queryParameters, token);

        public async Task<IReadOnlyList<MetadataRecord>> Search(string sqlQuery, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken token = default) =>
            await _metadataContainer.Search.Query(sqlQuery, parameters, token);
    }
}
