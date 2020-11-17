using PathFinder.sdk.Models;
using PathFinder.sdk.Services.RecordAbstract;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.sdk.Store
{
    public interface IRecordContainer<T> where T : IRecord
    {
        string ContainerName { get; }

        ISearchContainer<T> Search { get; }

        Task<bool> Delete(string id, string? eTag = null, string? partitionKey = null, CancellationToken token = default);

        Task<bool> Exist(string id, CancellationToken token = default);

        Task<Record<T>?> Get(string id, string? partitionKey = null, CancellationToken token = default);

        Task<ETag> Set(Record<T> record, CancellationToken token = default);

        Task<ETag> Set(T item, CancellationToken token = default);
    }
}