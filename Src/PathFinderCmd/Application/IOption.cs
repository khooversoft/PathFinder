using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Models;
using Toolbox.Services;

namespace PathFinderCmd.Application
{
    internal interface IOption
    {
        bool Link { get; }
        bool Clear { get; }
        string? ConfigFile { get; }
        bool Import { get; }
        bool Delete { get; }
        string? File { get; }
        bool Get { get; }
        bool Help { get; }
        string? Id { get; }
        bool Initialize { get; }
        KeyVaultOption? KeyVault { get; }
        bool List { get; }
        string? LogFolder { get; }
        IPropertyResolver PropertyResolver { get; }
        ISecretFilter SecretFilter { get; }
        string? SecretId { get; }
        CosmosPathFinderOption Store { get; }
        bool Metadata { get; }
        bool Template { get; }
    }
}