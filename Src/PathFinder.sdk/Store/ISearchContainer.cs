using PathFinder.sdk.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.sdk.Store
{
    public interface ISearchContainer<T> where T : IRecord
    {
        Task<IReadOnlyList<T>> List(QueryParameters queryParameters, CancellationToken token = default);

        Task<IReadOnlyList<T>> Query(string sqlQuery, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken token = default);
    }
}