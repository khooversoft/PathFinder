using System;
using System.Collections.Generic;
using System.Text;

namespace PathFinder.sdk.Models
{
    public class BatchSet<T>
    {
        public string ContinuationUrl { get; set; } = null!;

        public IReadOnlyList<T> Records { get; set; } = null!;
    }
}
