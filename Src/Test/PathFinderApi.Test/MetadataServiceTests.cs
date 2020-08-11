using FluentAssertions;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinderApi.Test.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PathFinderApi.Test
{
    public class MetadataServiceTests : IClassFixture<TestApplication>
    {
        private readonly TestApplication _testApplication;

        public MetadataServiceTests(TestApplication testApplication)
        {
            _testApplication = testApplication;
        }

        [Fact]
        public async Task GivenMetadataRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = _testApplication.GetHost();

            const string id = "meta0001";

            var record = new MetadataRecord
            {
                Id = id,
                Properties = new Dictionary<string, string>
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                }
            };

            await host.PathFinderClient.Metadata.Delete(record.Id);
            MetadataRecord? response = await host.PathFinderClient.Metadata.Get(record.Id);
            response.Should().BeNull();

            await host.PathFinderClient.Metadata.Set(record);
            MetadataRecord? readResponse = await host.PathFinderClient.Metadata.Get(record.Id);
            readResponse.Should().NotBeNull();

            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List();
            list.Should().NotBeNull();
            list.Records.Count.Should().BeGreaterThan(0);

            await host.PathFinderClient.Metadata.Delete(record.Id);

            readResponse = await host.PathFinderClient.Metadata.Get(record.Id);
            readResponse.Should().BeNull();
        }

        [Fact]
        public async Task GivenMultiMetadataRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = _testApplication.GetHost();
            await DeleteAll(host);

            const int max = 100;

            IReadOnlyList<MetadataRecord> records = Enumerable.Range(0, max)
                .Select(x => new MetadataRecord
                {
                    Id = $"meta{x}",
                    Properties = new Dictionary<string, string>
                    {
                        ["key1"] = "value1",
                        ["key2"] = "value2",
                    }
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Metadata.Set(item);
            }

            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List();
            list.Should().NotBeNull();
            list.Records.Count.Should().Be(max);

            records
                .Zip(list.Records, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();

            await DeleteAll(host);
        }

        private async Task DeleteAll(TestWebsiteHost host)
        {
            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List();

            foreach (var item in list.Records)
            {
                await host.PathFinderClient.Metadata.Delete(item.Id);
            }
        }
    }
}
