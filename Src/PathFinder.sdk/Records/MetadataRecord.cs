using Newtonsoft.Json;
using PathFinder.sdk.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace PathFinder.sdk.Records
{
    public class MetadataRecord : RecordBase, IRecord
    {
        public MetadataRecord() : base(nameof(MetadataRecord)) { }

        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        public IDictionary<string, string>? Properties { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public bool Enabled { get; set; } = true;

        public void Prepare()
        {
            Id = Id
                .VerifyNotEmpty(nameof(Id))
                .ToLowerInvariant();

            Properties
                .VerifyNotNull(nameof(Properties));
        }

        public override bool Equals(object? obj)
        {
            return obj is MetadataRecord record &&
                   Id?.ToLowerInvariant() == record.Id?.ToLowerInvariant() &&
                   IsPropertiesMatch(Properties, record.Properties) &&
                   Enabled == record.Enabled;

            static bool IsPropertiesMatch(IDictionary<string, string>? lValue, IDictionary<string, string>? rValue)
            {
                if (lValue == null && rValue == null) return true;

                return Enumerable.SequenceEqual(
                    lValue.Select(x => (Key: x.Key.ToLowerInvariant(), Value: x.Value)),
                    rValue.Select(x => (Key: x.Key.ToLowerInvariant(), Value: x.Value)));            }
        }

        public override int GetHashCode() => HashCode.Combine(Id);

        public override string ToString() => $"Id={Id}, Properties={string.Join(", ", Properties.Select(x => $"{x.Key}={x.Value}"))}";

        private string BuildPropertyString() => ((IEnumerable<KeyValuePair<string, string>>)Properties! ?? Array.Empty<KeyValuePair<string, string>>())
            .Select(x => $"{x.Key}={x.Value}")
            .Func(x => string.Join(", ", x));

        public static bool operator ==(MetadataRecord? left, MetadataRecord? right) => EqualityComparer<MetadataRecord>.Default.Equals(left!, right!);

        public static bool operator !=(MetadataRecord? left, MetadataRecord? right) => !(left == right);
    }
}
