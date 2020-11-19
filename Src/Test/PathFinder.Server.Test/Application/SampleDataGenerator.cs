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
    public class SampleDataGenerator
    {
        //[Fact]
        public async Task GivenMultiLinkRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = await TestApplication.DevHost.GetHost();
            await DeleteAll(host);

            int half = TestData.RandomNames.Count / 2;
            IReadOnlyList<LinkRecord> records = TestData.RandomNames.Take(half)
                .Zip(TestData.RandomNames.Skip(half), (name, site) => (name, site))
                .Select((x, i) => new LinkRecord
                {
                    Id = x.name,
                    RedirectUrl = $"http://{x.site}/Document",
                    Owner = $"Owner_{i % 5}",
                    Tags = Enumerable.Range(0, i % 3)
                        .Select(x => new KeyValue($"Key_{x}", $"Value_{x}"))
                        .ToList(),
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Link.Set(item);
            }

            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(QueryParameters.Default).ReadNext();
            list.Should().NotBeNull();
            list.Records.Count.Should().Be(half);

            records
                .Zip(list.Records, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        private async Task DeleteAll(TestWebsiteHost host)
        {
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(QueryParameters.Default).ReadNext();

            foreach (var item in list.Records)
            {
                await host.PathFinderClient.Link.Delete(item.Id);
            }
        }
   }
}