using System;
using System.Collections.Generic;
using System.Text;

namespace PathFinder.sdk.Models
{
    public interface IRecord : IRecordPrepare
    {
        string Id { get; }

        bool Equals(object? obj);
    }
}
