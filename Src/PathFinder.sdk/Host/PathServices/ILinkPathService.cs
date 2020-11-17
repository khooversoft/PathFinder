using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.sdk.Host.PathServices
{
    public interface ILinkPathService
    {
        string Name { get; }

        Task Delete(string id, CancellationToken token = default);

        Task<LinkRecord?> Get(string id, CancellationToken token = default);

        Task<IReadOnlyList<LinkRecord>> List(QueryParameters queryParameters, CancellationToken token = default);

        Task<IReadOnlyList<LinkRecord>> Search(string sqlQuery, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken token = default);

        Task Set(LinkRecord record, CancellationToken token = default);
    }
}