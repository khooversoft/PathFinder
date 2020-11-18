using PathFinderWeb.Client.Application.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathFinderWeb.Client.Application
{
    internal static class IconHelper
    {
        private const string _normalCode = "iconic-black";
        private const string _disableCode = "iconic-gray";
        private const string _greenCode = "iconic-green";
        private const string _blueCode = "iconic-blue";
        private const string _redCode = "iconic-red";

        public static Icon Home { get; } = new Icon("oi-home", _normalCode, _normalCode);
        public static Icon External { get; } = new Icon("oi-external-link", _normalCode, _normalCode);
        public static Icon List { get; } = new Icon("oi-list", _normalCode, _normalCode);
        public static Icon Create { get; } = new Icon("oi-pencil ", _blueCode, _disableCode);
        public static Icon Reload { get; } = new Icon("oi-reload", _normalCode, _normalCode);
        public static Icon Reset { get; } = new Icon("oi-ban", _normalCode, _normalCode);

        public static Icon Cancel { get; } = new Icon("oi-x", _normalCode, _disableCode);
        public static Icon Add { get; } = new Icon("oi-plus ", _blueCode, _disableCode);
        public static Icon Save { get; } = new Icon("oi-file ", _blueCode, _disableCode);
        public static Icon Delete { get; } = new Icon("oi-circle-x ", _redCode, _disableCode);
    }
}
