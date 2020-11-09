using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinderWeb.Client.Application.Menu
{
    public class MenuItem : IMenuItem
    {
        public MenuItem(string text, string href, Icon icon, bool enabled)
        {
            Text = text;
            Href = href;
            Icon = icon;
            Enabled = enabled;
        }

        public MenuItem(string text, Icon icon, string href, MenuItem[] children)
        {
            Text = text;
            Href = href;
            Icon = icon;
            Enabled = true;

            Children = children;
        }

        public string Text { get; }
        public string Href { get; }
        public Icon Icon { get; }
        public bool Enabled { get; }

        public MenuItem[]? Children { get; set; }
    }
}
