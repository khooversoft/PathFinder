using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Toolbox.Extensions;

namespace PathFinder.sdk.Services.RecordAbstract
{
    public class RecordBase
    {
        public RecordBase() { }

        protected RecordBase(string recordType) => RecordType = recordType;

        protected RecordBase(RecordBase recordBase)
        {
            RecordType = recordBase.RecordType;
            Owner = recordBase.Owner;

            Tags = recordBase.Tags
                ?.Where(x => !x.Key.IsEmpty() && !x.Value.IsEmpty())
                ?.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                ?.Select(x => x.First())
                ?.ToList() ?? Tags;
        }

        public string RecordType { get; set; } = null!;

        public IList<Tag> Tags { get; set; } = new List<Tag>();

        public string? Owner { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RecordBase subject &&
                   RecordType == subject.RecordType &&
                   Tags.Count == subject.Tags.Count &&
                   !Tags.Except(subject.Tags).Any() &&
                   Owner == subject.Owner;
        }

        public override int GetHashCode() => HashCode.Combine(RecordType, Tags, Owner);

        public static bool operator ==(RecordBase? left, RecordBase? right) => EqualityComparer<RecordBase>.Default.Equals(left!, right!);

        public static bool operator !=(RecordBase? left, RecordBase? right) => !(left == right);
    }
}
