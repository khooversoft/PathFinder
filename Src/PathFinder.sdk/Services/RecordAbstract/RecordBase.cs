using System;
using System.Collections.Generic;
using System.Text;

namespace PathFinder.sdk.Services.RecordAbstract
{
    public class RecordBase
    {
        public RecordBase() { RecordType = null!; }

        protected RecordBase(string recordType) => RecordType = recordType;

        public string RecordType { get; set; }
    }
}
