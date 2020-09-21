using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinderWeb.Client.Application.Menu
{
    public interface IMenuItem
    {
        public bool Enabled { get; }
    }
}
