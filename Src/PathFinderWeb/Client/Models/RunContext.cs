using PathFinder.sdk.Records;
using PathFinderWeb.Client.Application;
using System.Collections.Generic;
using System.Linq;

namespace PathFinderWeb.Client.Pages
{
    public class RunContext<T>
    {
        public RunState RunState { get; private set; } = RunState.Startup;

        public int? Count { get; private set; }

        public IReadOnlyList<T>? Records { get; private set; }

        public string? ErrorMessage { get; private set; }

        public string? SearchByIdUrl { get; set; }

        public string? SearchByTag { get; set; }

        public string? SearchByOwner { get; set; }

        public void SetStartup() => Clear();

        public void SetMessages(int count, IEnumerable<T> records)
        {
            Clear();
            RunState = RunState.Result;
            Count = count;
            Records = records.ToList();
        }

        public void SetError(string errorMessage)
        {
            Clear();
            RunState = RunState.Error;
            ErrorMessage = errorMessage;
        }

        private void Clear()
        {
            RunState = RunState.Startup;
            Count = null;
            Records = null;
            ErrorMessage = null;
        }
    }
}
