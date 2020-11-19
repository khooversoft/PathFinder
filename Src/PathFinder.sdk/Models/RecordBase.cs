using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Extensions;

namespace PathFinder.sdk.Models
{
    public class RecordBase : IRecordPrepare
    {
        public RecordBase()
        {
        }

        protected RecordBase(string recordType) => RecordType = recordType;

        protected RecordBase(RecordBase recordBase)
        {
            RecordType = recordBase.RecordType;
            Owner = recordBase.Owner;
            Note = recordBase.Note;

            Tags = recordBase.Tags
                ?.Where(x => !x.Key.IsEmpty() && !x.Value.IsEmpty())
                ?.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                ?.Select(x => x.First())
                ?.ToList() ?? Tags;
        }

        public bool Enabled { get; set; } = true;
        public string? Note { get; set; }
        public string? Owner { get; set; }
        public string RecordType { get; set; } = null!;
        public IList<KeyValue> Tags { get; set; } = new List<KeyValue>();

        public override bool Equals(object? obj)
        {
            return obj is RecordBase subject &&
                RecordType == subject.RecordType &&
                Enabled == subject.Enabled &&
                Owner == subject.Owner &&
                Note == subject.Note &&
                Tags.Count == subject.Tags.Count &&
                !Tags.Except(subject.Tags).Any();
        }

        public override int GetHashCode() => HashCode.Combine(RecordType, Tags, Owner);

        public virtual void Prepare()
        {
            Tags = Tags
                ?.Where(x => !x.Key.IsEmpty() && !x.Value.IsEmpty())
                ?.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                ?.Select(x => x.First())
                ?.ToList() ?? new List<KeyValue>();
        }

        public static bool operator !=(RecordBase? left, RecordBase? right) => !(left == right);

        public static bool operator ==(RecordBase? left, RecordBase? right) => EqualityComparer<RecordBase>.Default.Equals(left!, right!);
    }
}