using PathFinder.sdk.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinderWeb.Server.Application
{
    internal class Option : IOption
    {
        public string? ConfigFile { get; set; }

        public string PathFinderApiUrl { get; set; } = null!;

        public string? SecretId { get; set; }

        public string Environment { get; set; } = "dev";

        public RunEnvironment RunEnvironment { get; set; } = RunEnvironment.Unknown;
    }
}
