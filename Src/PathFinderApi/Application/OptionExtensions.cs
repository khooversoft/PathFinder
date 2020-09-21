using PathFinder.sdk.Application;
using System;
using Toolbox.Tools;

namespace PathFinderApi.Application
{
    internal static class OptionExtensions
    {
        private const string baseId = "PathFinderApi.Configs";

        public static Option Verify(this Option option)
        {
            option.VerifyNotNull(nameof(option));

            option.Environment.VerifyNotEmpty(nameof(option.Environment));
            option.Environment.ConvertToEnvironment().VerifyAssert(x => x != RunEnvironment.Unknown, "Invalid environment");

            TestStore(option);

            return option;
        }

        public static string ConvertToResourceId(this RunEnvironment subject) => subject switch
        {
            RunEnvironment.Dev => $"{baseId}.dev-config.json",
            RunEnvironment.Acpt => $"{baseId}.acpt-config.json",
            RunEnvironment.Prod => $"{baseId}.prod-config.json",

            _ => throw new InvalidOperationException(),
        };

        private static bool TestStore(Option option)
        {
            option.Store.VerifyNotNull("Store options are not specified");
            option.Store?.ConnectionString.VerifyNotEmpty($"{nameof(option.Store)}:{nameof(option.Store.ConnectionString)} is required");
            option.Store?.AccountKey.VerifyNotEmpty($"{nameof(option.Store)}:{nameof(option.Store.AccountKey)} is required");
            option.Store?.DatabaseName.VerifyNotEmpty($"{nameof(option.Store)}:{nameof(option.Store.DatabaseName)} is required");

            return true;
        }
    }
}
