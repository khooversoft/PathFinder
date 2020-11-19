using FluentAssertions;
using Microsoft.Extensions.Logging;
using PathFinder.Cosmos.Store;
using PathFinder.sdk.Models;
using PathFinder.sdk.Store;
using PathFinderCmd.Application;
using PathFinderCmd.Test.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace PathFinderCmd.Test.Activities
{
    public class ActivityTestBase<T> where T : IRecord
    {
        private readonly ILoggerFactory _loggerFactory = new TestLoggerBuilder().Build();
        private readonly string _entityName;

        public ActivityTestBase(string entityName)
        {
            entityName.VerifyNotEmpty(nameof(entityName));

            _entityName = entityName;
        }

        protected async Task RunFullLifeCycleTests(Func<T> createRecord)
        {
            T record = createRecord();

            // Create Agent ID
            string file = TestTools.WriteResourceToTempFile(typeof(T).Name, record);

            await CreateRecord(record, file);
            await GetRecord(record, file);
            await DeleteRecord(record);
        }

        protected async Task RequestTemplateForEntity()
        {
            string file = TestTools.CreateTempFileName(typeof(T).Name);
            if (File.Exists(file)) File.Delete(file);

            try
            {
                var args = new[]
                {
                    _entityName,
                    "Template",
                    $"File={file}",
                };

                TestConfiguration testConfiguration = new TestConfiguration();
                string[] programArgs = testConfiguration.BuildArgs(args);

                await Program.Main(programArgs);

                file.VerifyAssert(x => File.Exists(x), x => $"File {x} does not exist");
            }
            finally
            {
                File.Delete(file);
            }
        }

        protected async Task TestClearCollection(Func<int, T> createRecord, int count)
        {
            IReadOnlyList<T> list = Enumerable.Range(0, count)
                .Select((x, i) => createRecord(i))
                .ToArray();

            // Clear records
            var args = new[]
            {
                _entityName,
                "Clear",
            };

            TestConfiguration testConfiguration = new TestConfiguration();
            IOption option = testConfiguration.GetOption(args);

            IPathFinderStore store = new CosmosPathFinderStore(option.Store, _loggerFactory);
            IRecordContainer<T> container = await store.Container.Create<T>();

            foreach (var record in list)
            {
                await container.Set(record);
            }

            IReadOnlyList<T> readRecords = await container.Search.List(QueryParameters.Default);
            readRecords.Should().NotBeNull();
            readRecords.Count.Should().Be(list.Count);

            // Execute clear
            string[] programArgs = testConfiguration.BuildArgs(args);
            await Program.Main(programArgs);

            // Verify
            IReadOnlyList<T> verifyReadRecords = await container.Search.List(QueryParameters.Default);
            verifyReadRecords.Should().NotBeNull();
            verifyReadRecords.Count.Should().Be(0);
        }

        protected async Task CreateRecord(T record, string file)
        {
            try
            {
                var args = new[]
                {
                    "Import",
                    $"File={file}"
                };

                TestConfiguration testConfiguration = new TestConfiguration();
                string[] programArgs = testConfiguration.BuildArgs(args);
                IOption option = testConfiguration.GetOption(args);

                await Program.Main(programArgs);

                IPathFinderStore store = new CosmosPathFinderStore(option.Store, _loggerFactory);
                IRecordContainer<T> container = await store.Container.Create<T>();

                Record<T>? read = await container.Get(record.Id);
                read.Should().NotBeNull();
                record.Equals(read!.Value).Should().BeTrue();
            }
            finally
            {
                File.Delete(file);
            }
        }

        protected async Task GetRecord(T record, string file)
        {
            try
            {
                var args = new[]
                {
                    _entityName,
                    "Get",
                    $"File={file}",
                    $"Id={record.Id}",
                };

                TestConfiguration testConfiguration = new TestConfiguration();
                string[] programArgs = testConfiguration.BuildArgs(args);

                await Program.Main(programArgs);

                file.VerifyAssert(x => File.Exists(x), x => $"File {x} does not exist");
                T read = Json.Default.Deserialize<T>(File.ReadAllText(file));
                record.Equals(read).Should().BeTrue();
            }
            finally
            {
                File.Delete(file);
            }
        }

        protected async Task DeleteRecord(T agentRecord)
        {
            var args = new[]
            {
                    _entityName,
                    "Delete",
                    $"Id={agentRecord.Id}",
                };

            TestConfiguration testConfiguration = new TestConfiguration();
            string[] programArgs = testConfiguration.BuildArgs(args);
            IOption option = testConfiguration.GetOption(args);

            await Program.Main(programArgs);

            IPathFinderStore store = new CosmosPathFinderStore(option.Store, _loggerFactory);
            IRecordContainer<T> container = await store.Container.Create<T>();

            Record<T>? read = await container.Get(agentRecord.Id);
            read.Should().BeNull();
        }
    }
}
