using PathFinder.sdk.Application;
using System;
using System.Text;

namespace PathFinder.Server.Test.Application
{
    public static class TestApplication
    {
        internal static TestHost DefaultHost { get; } = new TestHost(RunEnvironment.Local);

        internal static TestHost SearchHost { get; } = new TestHost(RunEnvironment.Local, "dev.pathFinder-test-search");

        internal static TestHost DevHost { get; } = new TestHost(RunEnvironment.Dev);

        public static void Shutdown()
        {
            DefaultHost.ShutdownHost();
            SearchHost.ShutdownHost();
            DevHost.ShutdownHost();
        }
    }
}
