using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinderWeb.Client.Application.Menu
{
    public class MenuButton : IMenuItem
    {
        public MenuButton(string text, Func<Task> onClick, Icon icon, bool enabled)
        {
            Text = text;
            OnClick = onClick;
            Icon = icon;
            Enabled = enabled;
        }

        public string Text { get; }
        public Func<Task> OnClick { get; }
        public Icon Icon { get; }
        public bool Enabled { get; }
    }
}
