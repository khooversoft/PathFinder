using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Models;
using Toolbox.Services;

namespace PathFinderApi.Application
{
    internal class Option : IOption
    {
        public Option() { }

        public string? ConfigFile { get; set; }
        public string? LogFolder { get; set; }
        public string? ContinuationHost { get; set; }

        public string? SecretId { get; set; }

        public CosmosPathFinderOption Store { get; set; } = null!;

        public KeyVaultOption KeyVault { get; set; } = null!;

        public IPropertyResolver PropertyResolver { get; set; } = null!;

        public ISecretFilter SecretFilter { get; set; } = null!;
    }
}

