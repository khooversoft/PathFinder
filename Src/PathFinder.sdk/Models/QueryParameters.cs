using System.Collections.Generic;

namespace PathFinder.sdk.Models
{
    public class QueryParameters : QueryWindow
    {
        public QueryParameters() { }

        public QueryParameters(QueryParameters subject)
            : base(subject)
        {
            Id = subject.Id;
            RedirectUrl = subject.RedirectUrl;
            Owner = subject.Owner;
            Tag = subject.Tag;
        }

        public string? Id { get; set; }

        public string? RedirectUrl { get; set; }

        public string? Owner { get; set; }

        public string? Tag { get; set; }

        public static QueryParameters Default { get; } = new QueryParameters();
    }
}