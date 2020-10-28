using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Services;
using Toolbox.Tools;

namespace PathFinder.Cosmos.Store.Application
{
    public static class CosmosPathFinderOptionExtensions
    {
        public static void Verify(this ICosmosPathFinderOption option)
        {
            option.ConnectionString.VerifyNotEmpty(nameof(option.ConnectionString));
            option.AccountKey.VerifyNotEmpty(nameof(option.AccountKey));
            option.DatabaseName.VerifyNotEmpty(nameof(option.DatabaseName));
        }

        public static string ResolvedConnectionString(this ICosmosPathFinderOption option)
        {
            var properties = new[]
            {
                new KeyValuePair<string, string>(nameof(option.AccountKey), option.AccountKey),
                new KeyValuePair<string, string>(nameof(option.DatabaseName), option.DatabaseName),
            };

            return new PropertyResolver(properties).Resolve(option.ConnectionString);
        }
    }
}
