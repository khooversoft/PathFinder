using FluentAssertions;
using PathFinder.sdk.Application;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinder.Server.Test.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PathFinder.Server.Test
{
    public class MetadataServiceSearchTests
    {
        public MetadataServiceSearchTests()
        {
            TestApplication.SearchHost.EnqueueState(nameof(MetadataServiceSearchTests), _ => CreateDataSet());
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedForId_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act / Assert
            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(new QueryParameters { Id = "Z" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(2);

            // Act / Assert
            list = await host.PathFinderClient.Metadata.List(new QueryParameters { Id = "na" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(8);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedOwner_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act
            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(new QueryParameters { Owner = "2" }).ReadNext();

            // Assert
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(10);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchForTag_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act
            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(new QueryParameters { Tag = "1" }).ReadNext();

            // Assert
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(16);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedBothIdAndRedirectUrlAndOwner_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act
            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(new QueryParameters { Id = "sa", Owner = "3" }).ReadNext();

            // Assert
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(14);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedAllFields_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act
            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(new QueryParameters { Id = "Z", Owner = "3", Tag = "2" }).ReadNext();

            // Assert
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(11);
        }

        private async Task CreateDataSet()
        {
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();
            await DeleteAll(host);

            int half = TestData.RandomNames.Count / 2;
            IReadOnlyList<MetadataRecord> records = TestData.RandomNames.Take(half)
                .Zip(TestData.RandomNames.Skip(half), (name, site) => (name, site))
                .Select((x, i) => new MetadataRecord
                {
                    Id = x.name,
                    Owner = $"Owner_{i % 5}",

                    Properties = Enumerable.Range(0, i % 3)
                        .Select(x => new KeyValue($"PropertyKey_{x}", $"PropertyValue_{x}"))
                        .ToList(),

                    Tags = Enumerable.Range(0, i % 3)
                        .Select(x => new KeyValue($"Key_{x}", $"Value_{x}"))
                        .ToList(),
                })
                .ToArray();

            foreach (var item in records)
            {
                await host.PathFinderClient.Metadata.Set(item);
            }
        }

        private async Task DeleteAll(TestWebsiteHost host)
        {
            BatchSet<MetadataRecord> list = await host.PathFinderClient.Metadata.List(QueryParameters.Default).ReadNext();

            foreach (var item in list.Records)
            {
                await host.PathFinderClient.Metadata.Delete(item.Id);
            }
        }
    }
}
