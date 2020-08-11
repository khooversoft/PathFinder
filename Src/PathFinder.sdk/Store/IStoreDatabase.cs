using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.sdk.Store
{
    public interface IStoreDatabase
    {
        Task Create(string databaseName, CancellationToken token = default);
        Task<bool> Delete(string databaseName, CancellationToken token = default);
    }
}