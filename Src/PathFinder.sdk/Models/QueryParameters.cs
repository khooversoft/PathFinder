using System.Collections.Generic;

namespace PathFinder.sdk.Models
{
    public class QueryParameters
    {
        public QueryParameters() { }

        public QueryParameters(QueryParameters subject)
        {
            Index = subject.Index;
            Count = subject.Count;
            Id = subject.Id;
            RedirectUrl = subject.RedirectUrl;
            Owner = subject.Owner;
            Tag = subject.Tag;
        }

        public int Index { get; set; } = 0;

        public int Count { get; set; } = 1000;

        public string? Id { get; set; }

        public string? RedirectUrl { get; set; }

        public string? Owner { get; set; }

        public string? Tag { get; set; }

        public static QueryParameters Default { get; } = new QueryParameters();

        public override string ToString() => $"{nameof(Index)}={Index}, {nameof(Count)}={Count}";
    }
}