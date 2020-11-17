using FluentAssertions;
using PathFinder.sdk.Application;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PathFinder.Server.Test.Application
{
    public class SampleDataGenerator : IClassFixture<TestApplication>
    {
        private readonly TestApplication _testApplication;

        public SampleDataGenerator(TestApplication testApplication)
        {
            _testApplication = testApplication;
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = _testApplication.GetHost(RunEnvironment.Dev);
            await DeleteAll(host);

            int half = TestData.RandomNames.Count / 2;
            IReadOnlyList<LinkRecord> records = TestData.RandomNames.Take(half)
                .Zip(TestData.RandomNames.Skip(half), (name, site) => (name, site))
                .Select((x, i) => new LinkRecord
                {
                    Id = x.name,
                    RedirectUrl = $"http://{x.site}/Document",
                    Owner = $"Owner_{i % 5}",
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.Query(QueryParameters.Default).ReadNext();
            list.Should().NotBeNull();
            list.Records.Count.Should().Be(half);

            records
                .Zip(list.Records, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();

            //await DeleteAll(host);
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