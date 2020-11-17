using Newtonsoft.Json;
using PathFinder.sdk.Services.RecordAbstract;
using System;
using System.Collections.Generic;
using Toolbox.Tools;

namespace PathFinder.sdk.Records
{
    public class LinkRecord : RecordBase, IRecord
    {
        public LinkRecord() : base(nameof(LinkRecord))
        {
        }

        public LinkRecord(LinkRecord linkRecord)
            : base(linkRecord)
        {
            Id = linkRecord.Id;
            RedirectUrl = linkRecord.RedirectUrl;
            Enabled = linkRecord.Enabled;
        }

        public bool Enabled { get; set; } = true;

        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        public string RedirectUrl { get; set; } = null!;

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
                   base.Equals(obj) &&
                   Id == record.Id &&
                   RedirectUrl == record.RedirectUrl &&
                   Enabled == record.Enabled;
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Id, RedirectUrl, Enabled) ^ base.GetHashCode();

        public static bool operator !=(LinkRecord? left, LinkRecord? right) => !(left == right);

        public static bool operator ==(LinkRecord? left, LinkRecord? right) => EqualityComparer<LinkRecord>.Default.Equals(left!, right!);
    }
}