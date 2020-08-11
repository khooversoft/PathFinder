using PathFinder.sdk.Records;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor.Host;

namespace PathFinder.sdk.Host.PathServices
{
    public interface IMetadataPathService
    {
        string Name { get; }

        Task Delete(string id, CancellationToken token = default);
        Task<MetadataRecord?> Get(string id, CancellationToken token = default);
        Task<IReadOnlyList<MetadataRecord>> ListAll();
        Task<IReadOnlyList<MetadataRecord>> Search(string sqlQuery, IEnumerable<KeyValuePair<string, string>>? parameters = null);
        Task Set(MetadataRecord record, CancellationToken token = default);
    }
}