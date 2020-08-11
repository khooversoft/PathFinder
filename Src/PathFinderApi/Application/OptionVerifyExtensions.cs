using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Tools;
using Toolbox.Extensions;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace PathFinderApi.Application
{
    internal static class OptionVerifyExtensions
    {
        private static bool TestStore(Option option)
        {
            option.Store.VerifyNotNull("Store options are not specified");
            option.Store?.ConnectionString.VerifyNotEmpty($"{nameof(option.Store)}:{nameof(option.Store.ConnectionString)} is required");
            option.Store?.AccountKey.VerifyNotEmpty($"{nameof(option.Store)}:{nameof(option.Store.AccountKey)} is required");
            option.Store?.DatabaseName.VerifyNotEmpty($"{nameof(option.Store)}:{nameof(option.Store.DatabaseName)} is required");

            return true;
        }

        private static Func<Option, bool>[] _tests = new Func<Option, bool>[]
        {
            x => TestStore(x),
        };

        public static Option Verify(this Option option)
        {
            option.VerifyNotNull(nameof(option));

            _tests
                .TakeWhile(x => !x(option))
                .Count()
                .VerifyAssert(x => x != _tests.Length, "Unknown command");

            return option;
        }
    }
}
