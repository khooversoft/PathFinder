using System;
using System.Collections.Generic;
using System.Text;

namespace PathFinder.sdk.Models
{
    public class KeyValue
    {
        public KeyValue() { }

        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string? Key { get; set; }

        public string? Value { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is KeyValue tag &&
                   Key == tag.Key &&
                   Value == tag.Value;
        }

        public override int GetHashCode() => HashCode.Combine(Key, Value);

        public static bool operator ==(KeyValue? left, KeyValue? right) => EqualityComparer<KeyValue>.Default.Equals(left!, right!);

        public static bool operator !=(KeyValue? left, KeyValue? right) => !(left == right);
    }
}
