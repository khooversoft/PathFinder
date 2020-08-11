using PathFinder.sdk.Records;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PathFinderCmd.Test.Activities
{
    public class MetdatadataActivityTests : ActivityTestBase<MetadataRecord>
    {
        public MetdatadataActivityTests()
            : base("Metadata")
        {
        }

        [Fact]
        public async Task TestTargetFullLiveCycle_ShouldSucceeded() => await RunFullLifeCycleTests(() => new MetadataRecord
        {
            Id = "Metadata_1",
            Properties = new Dictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
            }
        });

        [Fact]
        public async Task RequestTemplateForEntityTest() => await RequestTemplateForEntity();

        [Fact]
        public async Task ClearCollectionTest() => await TestClearCollection(x => new MetadataRecord
        {
            Id = $"Metadata_{x}",
            Properties = new Dictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
            }
        }, 10);

    }
}
