using FluentAssertions;
using Microsoft.Extensions.Logging;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinder.Server.Test.Application;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PathFinder.Server.Test
{
    public class LinkServiceSearchTests
    {
        public LinkServiceSearchTests()
        {
            TestApplication.GetLoggerFactory()
                .CreateLogger<LinkServiceSearchTests>()
                .LogInformation("Constructor");

            TestApplication.SearchHost.EnqueueState(nameof(LinkServiceSearchTests), _ => CreateDataSet());
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedForId_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act / Assert
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { Id = "Z" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(2);

            // Act / Assert
            list = await host.PathFinderClient.Link.List(new QueryParameters { Id = "na" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(8);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedRedirectUrl_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act / Assert
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { RedirectUrl = "ee" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(5);

            // Act / Assert
            list = await host.PathFinderClient.Link.List(new QueryParameters { RedirectUrl = "sa" }).ReadNext();
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(3);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedOwner_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { Owner = "2" }).ReadNext();

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
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { Tag = "1" }).ReadNext();

            // Assert
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(16);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedBothIdAndRedirectUrl_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { Id = "Z", RedirectUrl = "sa" }).ReadNext();

            // Assert
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(5);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedBothIdAndRedirectUrlAndOwner_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { Id = "Z", RedirectUrl = "sa", Owner = "3" }).ReadNext();

            // Assert
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(13);
        }

        [Fact]
        public async Task GivenMultiLinkRecord_WhenSearchedAllFields_ShouldComplete()
        {
            // Arrange
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();

            // Act
            BatchSet<LinkRecord> list = await host.PathFinderClient.Link.List(new QueryParameters { Id = "Z", RedirectUrl = "sa", Owner = "3", Tag = "2" }).ReadNext();

            // Assert
            list.Should().NotBeNull();
            list.Records.Should().NotBeNull();
            list.Records.Count.Should().Be(13);
        }

        private async Task CreateDataSet()
        {
            TestWebsiteHost host = await TestApplication.SearchHost.GetHost();
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