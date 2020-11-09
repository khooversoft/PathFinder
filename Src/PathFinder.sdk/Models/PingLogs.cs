using System.Collections.Generic;

namespace PathFinder.sdk.Models
{
    public class PingLogs
    {
        public int Count { get; set; }
        public IList<string>? Messages { get; set; }
        public string? Version { get; set; }
    }
}