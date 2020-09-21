using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinderWeb.Client.Application
{
    internal static class NavigationHelper
    {
        public static string LinkPage() => "Link";
        public static string NewLinkPage() => $"Link/edit";
        public static string EditLinkPage(string id) => $"Link/edit/{id}";
    }
}
