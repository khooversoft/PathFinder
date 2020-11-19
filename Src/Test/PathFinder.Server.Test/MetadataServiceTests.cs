using FluentAssertions;
using PathFinder.sdk.Client;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinder.Server.Test.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PathFinder.Server.Test
{
    public class MetadataServiceTests
    {
        [Fact]
        public async Task GivenMetadataRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = await TestApplication.DefaultHost.GetHost();

            const string id = "meta0001";

            var record = new MetadataRecord
            {
                Id = id,
                Properties = new[]
                {
                    new KeyValue("key1", "value1"),
                    new KeyValue("key2", "value2"),
                },
            };

            await host.PathFinderClient.Metadata.Delete(record.Id);
            MetadataRecord? response = await host.PathFinderClient.Metadata.Get(record.Id);
            response.Should().BeNull();

            await host.PathFinderClient.Metadata.Set(record);
            MetadataRecord? readResponse = await host.PathFinderClient.Metadata.Get(record.Id);
            readResponse.Should().NotBeNull();

            await host.PathFinderClient.Metadata.Delete(record.Id);

            readResponse = await host.PathFinderClient.Metadata.Get(record.Id);
            readResponse.Should().BeNull();
        }

        [Fact]
        public async Task GivenMultiMetadataRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = await TestApplication.DefaultHost.GetHost();

            const string testPrefix = "metadata-test01-";
            await DeleteAll(host, testPrefix);

            const int max = 100;

            IReadOnlyList<MetadataRecord> records = Enumerable.Range(0, max)
                .Select(x => new MetadataRecord
                {
                    Id = $"{testPrefix}meta{x}",
                    Properties = new[]
                    {
                        new KeyValue("key1", "value1"),
                        new KeyValue("key2", "value2"),
                    },
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Metadata.Set(item);
            }

            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(new QueryParameters { Id = testPrefix }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Count.Should().Be(max);

            records
                .Zip(list.Records, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }


        [Fact]
        public async Task GivenMultiLinkRecord_WhenManuallyPaged_ShouldComplete()
        {
            TestWebsiteHost host = await TestApplication.DefaultHost.GetHost();

            const string testPrefix = "metadata-test02-";
            await DeleteAll(host, testPrefix);

            const int max = 100;
            const int pageSize = 10;

            IReadOnlyList<MetadataRecord> records = Enumerable.Range(0, max)
                .Select(x => new MetadataRecord
                {
                    Id = $"{testPrefix}meta_{x}",
                    Properties = new[]
                    {
                        new KeyValue("key1", "value1"),
                        new KeyValue("key2", "value2"),
                    },
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Metadata.Set(item);
            }

            var aggList = new List<MetadataRecord>();

            int index = 0;
            while (aggList.Count < max)
            {
                BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(new QueryParameters { Id = testPrefix, Index = index, Count = pageSize }).ReadNext();
                list.Should().NotBeNull();
                list.Records.Count.Should().Be(pageSize);

                index += list.Records.Count;
                aggList.AddRange(list.Records);
            }

            aggList.Count.Should().Be(max);

            records
                .Zip(aggList, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();

            BatchSet<MetadataRecord> finalList = await host.PathFinderClient.Metadata.List(new QueryParameters { Id = testPrefix, Index = index, Count = pageSize }).ReadNext();
            finalList.Should().NotBeNull();
            finalList.Records.Count.Should().Be(0);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenPagedWithContinuationUrl_ShouldComplete()
        {
            TestWebsiteHost host = await TestApplication.DefaultHost.GetHost();

            const string testPrefix = "metadata-test03-";
            await DeleteAll(host, testPrefix);

            const int max = 100;
            const int pageSize = 10;

            IReadOnlyList<MetadataRecord> records = Enumerable.Range(0, max)
                .Select(x => new MetadataRecord
                {
                    Id = $"{testPrefix}meta_{x}",
                    Properties = new[]
                    {
                        new KeyValue("key1", "value1"),
                        new KeyValue("key2", "value2"),
                    },
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Metadata.Set(item);
            }

            var aggList = new List<MetadataRecord>();
            BatchSetCursor<MetadataRecord> cursor = host.PathFinderClient.Metadata.List(new QueryParameters { Id = testPrefix, Index = 0, Count = pageSize });

            while (aggList.Count < max)
            {
                BatchSet<MetadataRecord> list = await cursor.ReadNext();
                list.Should().NotBeNull();
                list!.Records.Count.Should().Be(pageSize);

                aggList.AddRange(list.Records);
            }

            aggList.Count.Should().Be(max);
            BatchSet<MetadataRecord> lastList = await cursor.ReadNext();
            lastList.Should().NotBeNull();
            lastList.Records.Should().NotBeNull();
            lastList.Records.Count.Should().Be(0);

            records
                .Zip(aggList, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        private async Task DeleteAll(TestWebsiteHost host, string prefix)
        {
            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(new QueryParameters { Id = prefix }).ReadNext();

            foreach (var item in list.Records)
            {
                (await host.PathFinderClient.Metadata.Delete(item.Id)).Should().BeTrue(item.Id);
            }
        }
    }
}
