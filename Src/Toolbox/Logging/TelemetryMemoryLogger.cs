using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Services;
using Toolbox.Tools;

namespace Toolbox.Logging
{
    public class TelemetryMemoryLogger : ILogger
    {
        private readonly string _category;
        private readonly ITelemetryMemory _memoryTelemetry;

        public TelemetryMemoryLogger(string category, ITelemetryMemory telemetryMemory)
        {
            category.VerifyNotEmpty(nameof(category));
            telemetryMemory.VerifyNotNull(nameof(telemetryMemory));

            _category = category;
            _memoryTelemetry = telemetryMemory;
        }

        public IDisposable BeginScope<TState>(TState state) => null!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _memoryTelemetry.Add($"[{_category}] " + formatter(state, exception));
        }
    }
}
