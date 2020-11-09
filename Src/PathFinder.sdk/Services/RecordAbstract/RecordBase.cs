using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PathFinder.sdk.Services.RecordAbstract
{
    public class RecordBase
    {
        public RecordBase() { }

        protected RecordBase(string recordType) => RecordType = recordType;

        protected RecordBase(RecordBase recordBase)
        {
            RecordType = recordBase.RecordType;
            Tags = recordBase.Tags?.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase) ?? Tags;
        }

        public string RecordType { get; set; } = null!;

        public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
