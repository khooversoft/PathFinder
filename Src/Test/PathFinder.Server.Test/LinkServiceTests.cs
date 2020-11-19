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
    public class LinkServiceTests
    {
        [Fact]
        public async Task GivenLinkRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = await TestApplication.DefaultHost.GetHost();

            const string id = "lnk0001";
            const string redirectUrl = "http://localhost:5003/Document";

            var record = new LinkRecord
            {
                Id = id,
                RedirectUrl = redirectUrl
            };

            await host.PathFinderClient.Link.Delete(record.Id);
            LinkRecord? response = await host.PathFinderClient.Link.Get(record.Id);
            response.Should().BeNull();

            await host.PathFinderClient.Link.Set(record);
            LinkRecord? readResponse = await host.PathFinderClient.Link.Get(record.Id);
            readResponse.Should().NotBeNull();
            (record == readResponse).Should().BeTrue();

            await host.PathFinderClient.Link.Delete(record.Id);

            readResponse = await host.PathFinderClient.Link.Get(record.Id);
            readResponse.Should().BeNull();
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = await TestApplication.DefaultHost.GetHost();

            const string testPrefix = "test01-";
            await DeleteAll(host, testPrefix);

            const int max = 100;
            const string redirectUrl = "http://localhost:5003/Document";

            IReadOnlyList<LinkRecord> records = Enumerable.Range(0, max)
                .Select(x => new LinkRecord
                {
                    Id = $"{testPrefix}lnk_{x}",
                    RedirectUrl = redirectUrl,
                    Owner = $"Owner_{x}",
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { Id = testPrefix }).ReadNext();
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

            const string testPrefix = "test02-";
            await DeleteAll(host, testPrefix);

            const int max = 100;
            const int pageSize = 10;
            const string redirectUrl = "http://localhost:5003/Document";

            IReadOnlyList<LinkRecord> records = Enumerable.Range(0, max)
                .Select(x => new LinkRecord
                {
                    Id = $"{testPrefix}lnk_{x}",
                    RedirectUrl = redirectUrl
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            var aggList = new List<LinkRecord>();

            int index = 0;
            while (aggList.Count < max)
            {
                BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { Id = testPrefix, Index = index, Count = pageSize }).ReadNext();
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

            BatchSet<LinkRecord> finalList = await host.PathFinderClient.Link.List(new QueryParameters { Id = testPrefix, Index = index, Count = pageSize }).ReadNext();
            finalList.Should().NotBeNull();
            finalList.Records.Count.Should().Be(0);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenPagedWithContinuationUrl_ShouldComplete()
        {
            TestWebsiteHost host = await TestApplication.DefaultHost.GetHost();

            const string testPrefix = "test03-";
            await DeleteAll(host, testPrefix);

            const int max = 100;
            const int pageSize = 10;
            const string redirectUrl = "http://localhost:5003/Document";

            IReadOnlyList<LinkRecord> records = Enumerable.Range(0, max)
                .Select(x => new LinkRecord
                {
                    Id = $"{testPrefix}lnk_{x}",
                    RedirectUrl = redirectUrl
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            var aggList = new List<LinkRecord>();
            BatchSetCursor<LinkRecord> cursor = host.PathFinderClient.Link.List(new QueryParameters { Id = testPrefix, Index = 0, Count = pageSize });

            while (aggList.Count < max)
            {
                BatchSet<LinkRecord> list = await cursor.ReadNext();
                list.Should().NotBeNull();
                list!.Records.Count.Should().Be(pageSize);

                aggList.AddRange(list.Records);
            }

            aggList.Count.Should().Be(max);
            BatchSet<LinkRecord> lastList = await cursor.ReadNext();
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
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { Id = prefix }).ReadNext();

            foreach (var item in list.Records)
            {
                (await host.PathFinderClient.Link.Delete(item.Id)).Should().BeTrue(item.Id);
            }
        }
    }
}
