using System.Collections.Generic;

namespace Toolbox.Services
{
    public interface ITelemetryMemory
    {
        void Add(string message);
        IReadOnlyList<string> GetLoggedMessages();
    }
}