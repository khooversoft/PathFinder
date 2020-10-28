using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Services;
using Toolbox.Tools;

namespace PathFinder.Cosmos.Store.Application
{
    public class CosmosPathFinderOption : ICosmosPathFinderOption
    {
        /// <summary>
        /// Connection string for Cosmos DB account.  The string should use placeholders for the account key.
        /// 
        /// Example: "AccountEndpoint=https://test-db.documents.azure.com:443/;AccountKey={AccountKey};",
        /// </summary>
        public string ConnectionString { get; set; } = null!;

        /// <summary>
        /// Account key secret, should be loaded from a secret file, key vault or specified outside source code paths.
        /// </summary>
        public string AccountKey { get; set; } = null!;

        /// <summary>
        /// Trace collection Time to Live default value.  Records older then this will be removed by Cosmos
        /// </summary>
        public TimeSpan TraceTraceTTL { get; set; } = TimeSpan.FromDays(30);

        /// <summary>
        /// Database name
        /// </summary>
        public string DatabaseName { get; set; } = null!;

        /// <summary>
        /// Agent off-line tolerance time period.  Any agent that has not recorded a heart beat within
        /// this tolerance will be considered off-line and not available for load balancing.
        /// </summary>
        public TimeSpan OfflineTolerance { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Frequency of the agent's heart beat
        /// </summary>
        public TimeSpan HeartbeatFrequency { get; set; } = TimeSpan.FromMinutes(5);
    }
}
