using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.sdk.Host.PathServices
{
    public interface IMetadataPathService
    {
        string Name { get; }

        Task Delete(string id, CancellationToken token = default);

        Task<MetadataRecord?> Get(string id, CancellationToken token = default);

        Task<IReadOnlyList<MetadataRecord>> List(QueryParameters queryParameters, CancellationToken token = default);

        Task<IReadOnlyList<MetadataRecord>> Search(string sqlQuery, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken token = default);

        Task Set(MetadataRecord record, CancellationToken token = default);
    }
}