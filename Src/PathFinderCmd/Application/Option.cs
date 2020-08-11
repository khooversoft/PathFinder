using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Models;
using Toolbox.Services;

namespace PathFinderCmd.Application
{
    internal class Option : IOption
    {
        public Option() { }

        public bool Help { get; set; }
        public string? ConfigFile { get; set; }
        public string? LogFolder { get; set; }
        public bool Initialize { get; set; }
        public bool Import { get; set; }

        public bool Get { get; set; }
        public bool List { get; set; }
        public bool Delete { get; set; }
        public bool Clear { get; set; }
        public bool Template { get; set; }

        public bool Link { get; set; }
        public bool Metadata { get; set; }

        public string? SecretId { get; set; }

        public string? File { get; set; }
        public string? Id { get; set; }

        public CosmosPathFinderOption Store { get; set; } = null!;

        public KeyVaultOption? KeyVault { get; set; }

        public IPropertyResolver PropertyResolver { get; set; } = null!;

        public ISecretFilter SecretFilter { get; set; } = null!;
    }
}

