using Newtonsoft.Json;
using PathFinder.sdk.Services.RecordAbstract;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Tools;

namespace PathFinder.sdk.Records
{
    public class LinkRecord : RecordBase, IRecord
    {
        public LinkRecord() : base(nameof(LinkRecord)) { }

        public LinkRecord(LinkRecord linkRecord)
            : base(linkRecord)
        {
            Id = linkRecord.Id;
            RedirectUrl = linkRecord.RedirectUrl;
            Enabled = linkRecord.Enabled;
        }

        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        public string RedirectUrl { get; set; } = null!;

        public bool Enabled { get; set; } = true;

        public void Prepare()
        {
            Id = Id
                .VerifyNotEmpty(nameof(Id))
                .ToLowerInvariant();

            RedirectUrl.VerifyNotEmpty(nameof(RedirectUrl));
        }

        public override string ToString() => $"Id={Id}, RedirectUrl={RedirectUrl}, Enabled={Enabled}";

        public override bool Equals(object? obj)
        {
            return obj is LinkRecord record &&
                   Id.ToLowerInvariant() == record.Id.ToLowerInvariant() &&
                   RedirectUrl == record.RedirectUrl &&
                   Enabled == record.Enabled;
        }

        public override int GetHashCode() => HashCode.Combine(Id, RedirectUrl, Enabled);

        public static bool operator ==(LinkRecord? left, LinkRecord? right) => EqualityComparer<LinkRecord>.Default.Equals(left!, right!);

        public static bool operator !=(LinkRecord? left, LinkRecord? right) => !(left == right);
    }
}
