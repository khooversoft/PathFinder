using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace PathFinderWeb.Client.Application.Menu
{
    public class Icon
    {
        public Icon(string name, string enableCode, string disableCode)
        {
            name.VerifyNotEmpty(nameof(name));
            enableCode.VerifyNotEmpty(nameof(enableCode));
            disableCode.VerifyNotEmpty(nameof(disableCode));

            Name = name;
            EnableCode = enableCode;
            DisableCode = disableCode;
        }

        public string Name { get; }

        public string EnableCode { get; }

        public string DisableCode { get; }

        public string ToCode(bool enable) => Name + " " + (enable ? EnableCode : DisableCode);
    }
}
