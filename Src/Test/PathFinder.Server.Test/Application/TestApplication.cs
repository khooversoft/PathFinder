using Microsoft.Extensions.Logging;
using PathFinder.sdk.Application;
using System;
using System.Text;

namespace PathFinder.Server.Test.Application
{
    public static class TestApplication
    {
        private static ILoggerFactory? _loggerFactory;

        internal static TestHost DefaultHost { get; } = new TestHost(GetLoggerFactory(), RunEnvironment.Local);

        internal static TestHost SearchHost { get; } = new TestHost(GetLoggerFactory(), RunEnvironment.Local, "dev.pathFinder-test-search");

        internal static TestHost DevHost { get; } = new TestHost(GetLoggerFactory(), RunEnvironment.Dev);

        public static void Shutdown()
        {
            DefaultHost.ShutdownHost();
            SearchHost.ShutdownHost();
            DevHost.ShutdownHost();
        }

        public static ILoggerFactory GetLoggerFactory() => _loggerFactory ??= LoggerFactory.Create(x =>
        {
            x.AddConsole();
            x.AddDebug();
        });
    }
}
