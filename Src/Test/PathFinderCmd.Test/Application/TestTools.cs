﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Toolbox.Services;
using Toolbox.Tools;
using Toolbox.Extensions;

namespace PathFinderCmd.Test.Application
{
    public static class TestTools
    {
        public static Stream GetOptionStream(string id) => Assembly
            .GetAssembly(typeof(TestConfiguration))!
            .GetManifestResourceStream(id)
            .VerifyNotNull($"{id} is not an assembly resource");

        public static string[] GetArguments(string streamId, params string[] args) =>
            GetArguments(GetOptionStream(streamId), args);

        public static string[] GetArguments(Stream configStream, params string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .AddUserSecrets("PathFinderCmd")
                .AddCommandLine(args.Select(x => x.Split('=', StringSplitOptions.RemoveEmptyEntries).Length == 1 ? x += "=true" : x).ToArray())
                .Build();

            var argMap = new List<string>();
            var sectionQueue = new Queue<string?>(new string?[] { null });

            while (sectionQueue.Count > 0)
            {
                IEnumerable<IConfigurationSection> sections = sectionQueue.Dequeue() switch
                {
                    string v => config.GetSection(v).GetChildren(),
                    _ => config.GetChildren(),
                };

                foreach (IConfigurationSection item in sections)
                {
                    if (item.Value == null)
                    {
                        sectionQueue.Enqueue(item.Path);
                        continue;
                    }

                    argMap.Add($"{item.Path}={item.Value}");
                }
            }

            return argMap.ToArray();
        }

        public static string WriteResourceToTempFile<T>(string fileName, T value)
        {
            fileName.VerifyNotEmpty(nameof(fileName));
            value.VerifyNotNull(nameof(value));

            string file = CreateTempFileName(fileName);
            File.WriteAllText(file, Json.Default.Serialize(value));

            return file;
        }

        public static string CreateTempFileName(string fileName)
        {
            string file = Path.Combine(Path.GetTempPath(), "PathFinderCmd.Test", Path.ChangeExtension(fileName, ".json"));
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            return file;
        }
    }
}
