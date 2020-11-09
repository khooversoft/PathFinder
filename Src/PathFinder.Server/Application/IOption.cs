using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Application;
using PathFinder.sdk.Models;
using Toolbox.Services;

namespace PathFinder.Server.Application
{
    internal interface IOption
    {
        string? ConfigFile { get; }
        string? ContinuationHost { get; }
        string? LogFolder { get; }
        string[]? ApplicationUrl { get; }
        string? SecretId { get; }
        string Environment { get; }
        RunEnvironment RunEnvironment { get; }
        bool InitializeDatabase { get; }
        CosmosPathFinderOption Store { get; }
    }
}