using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Services
{
    public class TelemetryMemory : ITelemetryMemory
    {
        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private const int _maxMessages = 1000;

        public TelemetryMemory() { }

        public IReadOnlyList<string> GetLoggedMessages() => _queue
            .Select((x, i) => $"({i}) {x}")
            .ToList();

        public void Add(string message)
        {
            _queue.Enqueue(message);

            while (_queue.Count > _maxMessages) _queue.TryDequeue(out string _);
        }
    }
}
