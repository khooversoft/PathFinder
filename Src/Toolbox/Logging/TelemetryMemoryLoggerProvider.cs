using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Services;

namespace Toolbox.Logging
{
    public class TelemetryMemoryLoggerProvider : ILoggerProvider
    {
        private readonly ITelemetryMemory _memoryTelemetry;

        public TelemetryMemoryLoggerProvider(ITelemetryMemory telemetryMemory)
        {
            _memoryTelemetry = telemetryMemory;
        }

        public ILogger CreateLogger(string categoryName) => new TelemetryMemoryLogger(categoryName, _memoryTelemetry);

        public void Dispose() { }
    }
}
