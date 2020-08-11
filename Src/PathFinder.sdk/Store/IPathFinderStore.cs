using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.sdk.Store
{
    public interface IPathFinderStore
    {
        IStoreContainer Container { get; }
        IStoreDatabase Database { get; }

        Task InitializeContainers(CancellationToken token = default);
    }
}