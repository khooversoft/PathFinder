using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace PathFinder.sdk.Application
{
    public static class Extensions
    {
        private const string _recordText = "Record";

        public static RunEnvironment ConvertToEnvironment(this string subject)
        {
            Enum.TryParse(subject, true, out RunEnvironment enviornment)
                .VerifyAssert(x => x == true, $"Invalid environment {subject}");

            return enviornment;
        }

        public static string GetContainerName(this Type subject) => subject.Name
            .Func(x => x.EndsWith(_recordText) ? x.Substring(0, x.Length - _recordText.Length) : x);
    }
}
