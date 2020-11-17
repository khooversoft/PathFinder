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
    public class LinkServiceTests : IClassFixture<TestApplication>
    {
        private readonly TestApplication _testApplication;

        public LinkServiceTests(TestApplication testApplication)
        {
            _testApplication = testApplication;
        }

        [Fact]
        public async Task GivenLinkRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = _testApplication.GetHost();

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

            BatchSet<LinkRecord>? list = await host.PathFinderClient.Link.Query(QueryParameters.Default).ReadNext();
            list.Should().NotBeNull();
            list.Records.Count.Should().BeGreaterThan(0);

            await host.PathFinderClient.Link.Delete(record.Id);

            readResponse = await host.PathFinderClient.Link.Get(record.Id);
            readResponse.Should().BeNull();
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = _testApplication.GetHost();
            await DeleteAll(host);

            const int max = 100;
            const string redirectUrl = "http://localhost:5003/Document";

            IReadOnlyList<LinkRecord> records = Enumerable.Range(0, max)
                .Select(x => new LinkRecord
                {
                    Id = $"lnk_{x}",
                    RedirectUrl = redirectUrl,
                    Owner = $"Owner_{x}",
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.Query(QueryParameters.Default).ReadNext();
            list.Should().NotBeNull();
            list.Records.Count.Should().Be(max);

            records
                .Zip(list.Records, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();

            await DeleteAll(host);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenPaged_ShouldComplete()
        {
            TestWebsiteHost host = _testApplication.GetHost();
            await DeleteAll(host);

            const int max = 100;
            const int pageSize = 10;
            const string redirectUrl = "http://localhost:5003/Document";

            IReadOnlyList<LinkRecord> records = Enumerable.Range(0, max)
                .Select(x => new LinkRecord
                {
                    Id = $"lnk_{x}",
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
                BatchSet<LinkRecord> list = await host.PathFinderClient.Link.Query(new QueryParameters { Index = index, Count = pageSize }).ReadNext();
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

            BatchSet<LinkRecord> finalList = await host.PathFinderClient.Link.Query(new QueryParameters { Index = index, Count = pageSize }).ReadNext();
            finalList.Should().NotBeNull();
            finalList.Records.Count.Should().Be(0);

            await DeleteAll(host);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenPartialIdPaged_ShouldComplete()
        {
            TestWebsiteHost host = _testApplication.GetHost();
            await DeleteAll(host);

            int half = TestData.RandomNames.Count / 2;
            IReadOnlyList<LinkRecord> records = TestData.RandomNames.Take(half)
                .Zip(TestData.RandomNames.Skip(half), (name, site) => (name, site))
                .Select(x => new LinkRecord
                {
                    Id = x.name,
                    RedirectUrl = $"http://{x.site}/Document"
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.Query(new QueryParameters { Id = "Z" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(2);

            list = await host.PathFinderClient.Link.Query(new QueryParameters { Id = "i" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(32);

            list = await host.PathFinderClient.Link.Query(new QueryParameters { Id = "n" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(42);

            await DeleteAll(host);
        }


        [Fact]
        public async Task GivenMultiLinkRecord_WhenRedirectPaged_ShouldComplete()
        {
            TestWebsiteHost host = _testApplication.GetHost();
            await DeleteAll(host);

            int half = TestData.RandomNames.Count / 2;
            IReadOnlyList<LinkRecord> records = TestData.RandomNames.Take(half)
                .Zip(TestData.RandomNames.Skip(half), (name, site) => (name, site))
                .Select((x, i) => new LinkRecord
                {
                    Id = x.name,
                    RedirectUrl = $"http://{x.site}/Document",
                    Owner = $"Owner_{i % 5}"
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.Query(new QueryParameters { Id = "Z" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(2);

            list = await host.PathFinderClient.Link.Query(new QueryParameters { Id = "na" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(8);

            list = await host.PathFinderClient.Link.Query(new QueryParameters { RedirectUrl = "ee" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(5);

            list = await host.PathFinderClient.Link.Query(new QueryParameters { RedirectUrl = "sa" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(3);

            list = await host.PathFinderClient.Link.Query(new QueryParameters { Owner = "2" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(10);

            list = await host.PathFinderClient.Link.Query(new QueryParameters { Id = "Z", RedirectUrl = "sa" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(5);

            list = await host.PathFinderClient.Link.Query(new QueryParameters { Id = "Z", RedirectUrl = "sa", Owner = "3" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(13);

            await DeleteAll(host);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenPagedWithContinuationUrl_ShouldComplete()
        {
            TestWebsiteHost host = _testApplication.GetHost();
            await DeleteAll(host);

            const int max = 100;
            const int pageSize = 10;
            const string redirectUrl = "http://localhost:5003/Document";

            IReadOnlyList<LinkRecord> records = Enumerable.Range(0, max)
                .Select(x => new LinkRecord
                {
                    Id = $"lnk_{x}",
                    RedirectUrl = redirectUrl
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            var aggList = new List<LinkRecord>();
            BatchSetCursor<LinkRecord> cursor = host.PathFinderClient.Link.Query(new QueryParameters { Index = 0, Count = pageSize });

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

            await DeleteAll(host);
        }

        private async Task DeleteAll(TestWebsiteHost host)
        {
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.Query(QueryParameters.Default).ReadNext();

            foreach (var item in list.Records)
            {
                await host.PathFinderClient.Link.Delete(item.Id);
            }
        }
    }
}
