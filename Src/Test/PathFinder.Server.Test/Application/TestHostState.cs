using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.Server.Test.Application
{
    internal abstract class TestHostState
    {
        private readonly ConcurrentQueue<(string name, Func<TestHost, Task> func)> _stateQueue = new ConcurrentQueue<(string name, Func<TestHost, Task> func)>();
        private readonly HashSet<string> _executedStateNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly ILogger<TestHostState> _logger;
        private int _recursiveLock = 0;

        public TestHostState(ILogger<TestHostState> logger)
        {
            _logger = logger;
        }

        public void EnqueueState(string name, Func<TestHost, Task> func) => _stateQueue.Enqueue((name, func));

        protected async Task ExceuteQueuedState(TestHost testHost)
        {
            int currentLock = Interlocked.CompareExchange(ref _recursiveLock, 1, 0);
            if (currentLock == 1) return;

            try
            {
                while (_stateQueue.TryDequeue(out (string name, Func<TestHost, Task> func) queuedItem))
                {
                    if (_executedStateNames.Contains(queuedItem.name)) continue;
                    _executedStateNames.Add(queuedItem.name);

                    _logger.LogInformation($"{nameof(ExceuteQueuedState)}: name={queuedItem.name}");
                    await queuedItem.func(testHost);
                }
            }
            finally
            {
                _recursiveLock = 0;
            }
        }
    }
}
