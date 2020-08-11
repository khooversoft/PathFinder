using Microsoft.Extensions.Logging;
using PathFinder.sdk.Records;
using PathFinder.sdk.Store;
using PathFinderCmd.Application;
using Toolbox.Services;

namespace PathFinderCmd.Activities
{
    internal class MetadataActivity : ActivityEntityBase<MetadataRecord>
    {
        public MetadataActivity(IOption option, IRecordContainer<MetadataRecord> recordContainer, IJson json, ILogger<MetadataActivity> logger)
            : base(option, recordContainer, json, logger, "Metadata")
        {
        }
    }
}
