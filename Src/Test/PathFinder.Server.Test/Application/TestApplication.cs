using PathFinder.sdk.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.Server.Test.Application
{
    public class TestApplication
    {
        private static TestWebsiteHost? _currentHost;
        private static RunEnvironment? _currentEnvironment;
        private static object _lock = new object();

        internal TestWebsiteHost GetHost(RunEnvironment runEnvironment = RunEnvironment.Local)
        {
            lock (_lock)
            {
                if (runEnvironment != _currentEnvironment) ShutdownHost();

                _currentEnvironment = runEnvironment;
                return _currentHost = _currentHost ?? new TestWebsiteHost().StartApiServer(runEnvironment);
            }
        }

        internal void ShutdownHost() => Interlocked.Exchange(ref _currentHost, null)?.Shutdown();
    }
}
