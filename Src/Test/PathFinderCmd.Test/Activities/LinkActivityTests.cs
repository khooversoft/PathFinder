using PathFinder.sdk.Records;
using System.Threading.Tasks;
using Xunit;

namespace PathFinderCmd.Test.Activities
{
    public class LinkActivityTests : ActivityTestBase<LinkRecord>
    {
        public LinkActivityTests()
            : base("Link")
        {
        }

        [Fact]
        public async Task TestAgentFullLifeCycle_ShouldSucceeded() => await RunFullLifeCycleTests(() => new LinkRecord
        {
            Id = "link_1",
            RedirectUrl = "http://redirect"
        });

        [Fact]
        public async Task RequestTemplateForEntityTest() => await RequestTemplateForEntity();

        [Fact]
        public async Task ClearCollectionTest() => await TestClearCollection(x => new LinkRecord
        {
            Id = $"link_{x}",
            RedirectUrl = "http://redirect"
        }, 10);
    }
}
