using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinderApi.Test.Application
{
    public class TestApplication : IDisposable
    {
        private static TestWebsiteHost? _currentHost;
        private static object _lock = new object();

        internal TestWebsiteHost GetHost()
        {
            lock (_lock)
            {
                return _currentHost = _currentHost ?? new TestWebsiteHost().StartApiServer();
            }
        }

        public void Dispose() => Interlocked.Exchange(ref _currentHost, null!)?.Shutdown();
    }
}
