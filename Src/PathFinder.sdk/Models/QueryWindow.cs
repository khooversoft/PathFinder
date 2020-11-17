using System;
using System.Collections.Generic;
using System.Text;

namespace PathFinder.sdk.Models
{
    public class QueryWindow
    {
        public QueryWindow() { }

        public QueryWindow(QueryWindow queryWindow)
        {
            Index = queryWindow.Index;
            Count = queryWindow.Count;
        }

        public int Index { get; set; } = 0;

        public int Count { get; set; } = 1000;
    }
}
