using System;
using System.Collections.Generic;
using System.Text;

namespace PathFinder.sdk.Models
{
    public class BatchSet<T>
    {
        public QueryParameters QueryParameters { get; set; } = null!;

        public int NextIndex { get; set; }

        public IReadOnlyList<T> Records { get; set; } = null!;
    }
}
