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

            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List();
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
                    RedirectUrl = redirectUrl

                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List();
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
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List();

            foreach (var item in list.Records)
            {
                await host.PathFinderClient.Link.Delete(item.Id);
            }
        }
    }
}
