using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinderWeb.Client.Application
{
    internal static class NavigationHelper
    {
        public static class Link
        {
            public static string LinkPage() => "Link";
            public static string NewLinkPage() => $"Link/edit";
            public static string EditLinkPage(string id) => $"Link/edit/{id}";
        }

        public static class Metadata
        {
            public static string MetadataPage() => "Metadata";
            public static string NewMetadataPage() => $"Metadata/edit";
            public static string EditMetadataPage(string id) => $"Metadata/edit/{id}";
        }
    }
}
