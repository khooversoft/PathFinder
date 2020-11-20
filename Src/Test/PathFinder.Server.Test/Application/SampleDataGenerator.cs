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
            await DeleteAllLink(host);

            int half = TestData.RandomNames.Count / 2;
            IReadOnlyList<LinkRecord> records = TestData.RandomNames.Take(half)
                .Zip(TestData.RandomNames.Skip(half), (name, site) => (name, site))
                .Select((x, i) => new LinkRecord
                {
                    Id = $"link_{x.name}",
                    RedirectUrl = $"http://{x.site}/Document",
                    Owner = $"l-Owner_{i % 5}",
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

        //[Fact]
        public async Task GivenMultiMetadataRecord_WhenFullLifeCycle_ShouldComplete()
        {
            TestWebsiteHost host = await TestApplication.DevHost.GetHost();
            await DeleteAllMetadata(host);

            int half = TestData.RandomNames.Count / 2;
            IReadOnlyList<MetadataRecord> records = TestData.RandomNames.Take(half)
                .Zip(TestData.RandomNames.Skip(half), (name, site) => (name, site))
                .Select((x, i) => new MetadataRecord
                {
                    Id = $"meta_{x.name}",
                    Owner = $"m-Owner_{i % 5}",
                    Tags = Enumerable.Range(0, i % 3)
                        .Select(x => new KeyValue($"Key_{x}", $"Value_{x}"))
                        .ToList(),
                    Properties = Enumerable.Range(0, i % 5)
                        .Select(x => new KeyValue($"PropertyKey_{x}", $"PropertyValue_{x}"))
                        .ToList(),
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Metadata.Set(item);
            }

            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(QueryParameters.Default).ReadNext();
            list.Should().NotBeNull();
            list.Records.Count.Should().Be(half);

            records
                .Zip(list.Records, (o, i) => (o, i))
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        private async Task DeleteAllLink(TestWebsiteHost host)
        {
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(QueryParameters.Default).ReadNext();

            foreach (var item in list.Records)
            {
                await host.PathFinderClient.Link.Delete(item.Id);
            }
        }

        private async Task DeleteAllMetadata(TestWebsiteHost host)
        {
            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(QueryParameters.Default).ReadNext();

            foreach (var item in list.Records)
            {
                await host.PathFinderClient.Metadata.Delete(item.Id);
            }
        }
   }
}