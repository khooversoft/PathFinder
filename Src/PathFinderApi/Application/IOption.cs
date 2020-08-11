using PathFinder.Cosmos.Store.Application;
using PathFinder.sdk.Models;
using Toolbox.Services;

namespace PathFinderApi.Application
{
    internal interface IOption
    {
        string? ConfigFile { get; }
        string? ContinuationHost { get; }
        KeyVaultOption KeyVault { get; }
        string? LogFolder { get; }
        IPropertyResolver PropertyResolver { get; }
        ISecretFilter SecretFilter { get; }
        string? SecretId { get; }
        CosmosPathFinderOption Store { get; }
    }
}