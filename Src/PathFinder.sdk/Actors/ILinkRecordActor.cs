using PathFinder.sdk.Records;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;

namespace PathFinder.sdk.Actors
{
    public interface ILinkRecordActor : IActor
    {
        Task<bool> Delete(CancellationToken token);
        Task<LinkRecord?> Get(CancellationToken token);
        Task Set(LinkRecord linkRecord, CancellationToken token);
    }
}