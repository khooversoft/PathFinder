using Microsoft.Extensions.Logging;
using PathFinder.sdk.Records;
using PathFinder.sdk.Store;
using PathFinderCmd.Application;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Services;

namespace PathFinderCmd.Activities
{
    internal class LinkActivity : ActivityEntityBase<LinkRecord>
    {
        public LinkActivity(IOption option, IRecordContainer<LinkRecord> recordContainer, IJson json, ILogger<LinkActivity> logger)
            : base(option, recordContainer, json, logger, "Link")
        {
        }
    }
}
