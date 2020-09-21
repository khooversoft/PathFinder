using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Tools;

namespace PathFinder.sdk.Application
{
    public static class Extensions
    {
        public static RunEnvironment ConvertToEnvironment(this string subject)
        {
            Enum.TryParse(subject, true, out RunEnvironment enviornment)
                .VerifyAssert(x => x == true, $"Invalid environment {subject}");

            return enviornment;
        }
    }
}
