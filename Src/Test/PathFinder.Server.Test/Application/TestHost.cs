using PathFinder.sdk.Application;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.Server.Test.Application
{
    internal class TestHost : TestHostState
    {
        private TestWebsiteHost? _currentHost;
        private readonly object _lock = new object();
        private readonly RunEnvironment _runEnvironment;
        private readonly string? _databaseName;

        public TestHost(RunEnvironment runEnvironment, string? databaseName = null)
        {
            _runEnvironment = runEnvironment;
            _databaseName = databaseName;
        }

        public async Task<TestWebsiteHost> GetHost()
        {
            lock (_lock)
            {
                _currentHost ??= new TestWebsiteHost().StartApiServer(_runEnvironment, _databaseName);
            }

            await ExceuteQueuedState(this);

            return _currentHost;
        }

        public void ShutdownHost() => Interlocked.Exchange(ref _currentHost, null)?.Shutdown();
    }
}
