using PathFinder.sdk.Records;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor.Host;

namespace PathFinder.sdk.Host.PathServices
{
    public interface ILinkPathService
    {
        string Name { get; }

        Task Delete(string id, CancellationToken token = default);
        Task<LinkRecord?> Get(string id, CancellationToken token = default);
        Task<IReadOnlyList<LinkRecord>> ListAll();
        Task<IReadOnlyList<LinkRecord>> Search(string sqlQuery, IEnumerable<KeyValuePair<string, string>>? parameters = null);
        Task Set(LinkRecord record, CancellationToken token = default);
    }
}