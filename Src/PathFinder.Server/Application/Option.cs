using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Application;
using PathFinder.sdk.Models;
using Toolbox.Services;

namespace PathFinder.Server.Application
{
    internal class Option : IOption
    {
        public string? ConfigFile { get; set; }
        public string? LogFolder { get; set; }
        public string? ContinuationHost { get; set; }

        public string? SecretId { get; set; }

        public string[]? ApplicationUrl { get; set; }

        public string Environment { get; set; } = "local";

        public RunEnvironment RunEnvironment { get; set; } = RunEnvironment.Unknown;

        public bool InitializeDatabase { get; set; }

        public CosmosPathFinderOption Store { get; set; } = null!;
    }
}

