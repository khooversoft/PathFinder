using System;
using System.Collections.Generic;

namespace PathFinder.Cosmos.Store.Application
{
    public interface ICosmosPathFinderOption
    {
        string AccountKey { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }

        IReadOnlyList<KeyValuePair<string, string>> GetProperties();
        string GetResolvedConnectionString();
        string Resolve(string value);
        void Verify();
    }
}