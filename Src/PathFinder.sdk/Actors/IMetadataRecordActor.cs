using PathFinder.sdk.Records;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;

namespace PathFinder.sdk.Actors
{
    public interface IMetadataRecordActor : IActor
    {
        Task<bool> Delete(CancellationToken token);
        Task<MetadataRecord?> Get(CancellationToken token);
        Task Set(MetadataRecord record, CancellationToken token);
    }
}