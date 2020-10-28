using System;

namespace PathFinder.Cosmos.Store.Application
{
    public interface ICosmosPathFinderOption
    {
        string AccountKey { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        TimeSpan HeartbeatFrequency { get; set; }
        TimeSpan OfflineTolerance { get; set; }
        TimeSpan TraceTraceTTL { get; set; }
    }
}