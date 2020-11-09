using Toolbox.Tools;
using PathFinder.sdk.Application;
using System;

namespace PathFinderWeb.Server.Application
{
    internal static class OptionExtensions
    {
        private const string baseId = "PathFinderWeb.Server.Configs";

        public static void Verify(this Option option)
        {
            option.VerifyNotNull(nameof(option));
            option.PathFinderApiUrl.VerifyNotEmpty(nameof(option.PathFinderApiUrl));
            option.Environment.VerifyNotEmpty(nameof(option.Environment));
            option.Environment.ConvertToEnvironment().VerifyAssert(x => x != RunEnvironment.Unknown, "Invalid environment");
        }

        public static string ConvertToResourceId(this RunEnvironment subject) => subject switch
        {
            RunEnvironment.Dev => $"{baseId}.dev-config.json",
            RunEnvironment.Acpt => $"{baseId}.acpt-config.json",
            RunEnvironment.Prod => $"{baseId}.prod-config.json",

            _ => throw new InvalidOperationException(),
        };
    }
}
