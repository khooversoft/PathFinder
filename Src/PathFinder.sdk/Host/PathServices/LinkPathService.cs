using Microsoft.Extensions.DependencyInjection;
using PathFinder.sdk.Actors;
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
    public class LinkPathService : ILinkPathService
    {
        private readonly IRecordContainer<LinkRecord> _linkContainer;
        public IActorHost? _actorHost;

        public LinkPathService(IActorHost actorHost, IRecordContainer<LinkRecord> linkContainer)
        {
            _actorHost = actorHost;
            _linkContainer = linkContainer;
        }

        public string Name { get; } = nameof(ILinkPathService).ToLowerInvariant();

        public async Task<LinkRecord?> Get(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));

            ILinkRecordActor actor = _actorHost!.GetActor<ILinkRecordActor>((ActorKey)id);
            return await actor.Get(token);
        }

        public async Task Set(LinkRecord record, CancellationToken token = default)
        {
            record.VerifyNotNull(nameof(record));

            ILinkRecordActor actor = _actorHost!.GetActor<ILinkRecordActor>((ActorKey)record.Id);
            await actor.Set(record, token);
        }

        public async Task Delete(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));

            ILinkRecordActor actor = _actorHost!.GetActor<ILinkRecordActor>((ActorKey)id);
            await actor.Delete(token);
        }

        public async Task<IReadOnlyList<LinkRecord>> ListAll() => await _linkContainer.ListAll();

        public async Task<IReadOnlyList<LinkRecord>> Search(string sqlQuery, IEnumerable<KeyValuePair<string, string>>? parameters = null) => await _linkContainer.Search(sqlQuery, parameters);
    }
}
