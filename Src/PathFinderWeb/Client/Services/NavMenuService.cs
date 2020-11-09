using PathFinderWeb.Client.Application;
using PathFinderWeb.Client.Application.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinderWeb.Client.Services
{
    public class NavMenuService
    {
        public NavMenuService() { }

        public IReadOnlyList<MenuItem> GetLeftMenuItems() => new[]
        {
            new MenuItem("Home", string.Empty, IconHelper.Home, true),
            new MenuItem("Links", NavigationHelper.LinkPage(), IconHelper.External, true),
            new MenuItem("Metadata", "metadata", IconHelper.List, true),
        };
    }
}
