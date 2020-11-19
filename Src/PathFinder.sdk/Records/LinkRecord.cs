using Newtonsoft.Json;
using PathFinder.sdk.Models;
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

        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        public string RedirectUrl { get; set; } = null!;

        public override void Prepare()
        {
            base.Prepare();

            Id = Id
                .VerifyNotEmpty(nameof(Id))
                .ToLowerInvariant();

            RedirectUrl.VerifyNotEmpty(nameof(RedirectUrl));
        }

        public override string ToString() => $"Id={Id}, RedirectUrl={RedirectUrl}";

        public override bool Equals(object? obj)
        {
            return obj is LinkRecord record &&
                   base.Equals(obj) &&
                   Id == record.Id &&
                   RedirectUrl == record.RedirectUrl;
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Id, RedirectUrl, Enabled) ^ base.GetHashCode();

        public static bool operator !=(LinkRecord? left, LinkRecord? right) => !(left == right);

        public static bool operator ==(LinkRecord? left, LinkRecord? right) => EqualityComparer<LinkRecord>.Default.Equals(left!, right!);
    }
}