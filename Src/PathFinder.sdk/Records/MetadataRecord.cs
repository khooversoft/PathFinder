using Newtonsoft.Json;
using PathFinder.sdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace PathFinder.sdk.Records
{
    public class MetadataRecord : RecordBase, IRecord
    {
        public MetadataRecord()
            : base(nameof(MetadataRecord))
        {
        }

        public MetadataRecord(MetadataRecord metadataRecord)
            : this()
        {
            Id = metadataRecord.Id;

            Properties = metadataRecord.Properties
                ?.Where(x => !x.Key.IsEmpty() && !x.Value.IsEmpty())
                ?.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                ?.Select(x => x.First())
                ?.ToList() ?? Properties;
        }

        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        public IList<KeyValue> Properties { get; set; } = new List<KeyValue>();

        public override void Prepare()
        {
            base.Prepare();

            Id = Id
                .VerifyNotEmpty(nameof(Id))
                .ToLowerInvariant();

            Properties = Properties
                ?.Where(x => !x.Key.IsEmpty() && !x.Value.IsEmpty())
                ?.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                ?.Select(x => x.First())
                ?.ToList() ?? new List<KeyValue>();
        }

        public override bool Equals(object? obj)
        {
            return obj is MetadataRecord record &&
                base.Equals(obj) &&
                Id == record.Id &&
                Properties.Count == record.Properties.Count &&
                !Properties.Except(record.Properties).Any();
        }

        public override int GetHashCode() => HashCode.Combine(Id);

        public override string ToString() => $"Id={Id}, Properties={string.Join(", ", Properties.Select(x => $"{x.Key}={x.Value}"))}";

        public static bool operator ==(MetadataRecord? left, MetadataRecord? right) => EqualityComparer<MetadataRecord>.Default.Equals(left!, right!);

        public static bool operator !=(MetadataRecord? left, MetadataRecord? right) => !(left == right);
    }
}