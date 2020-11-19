using FluentAssertions;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System;
using System.Collections.Generic;
using Xunit;

namespace PathFinder.sdk.Test
{
    public class MetadataRecordEqutityTests
    {
        [Fact]
        public void GivenIdTests_AllShouldPass()
        {
            var variations = new[]
            {
                new {
                    Source = new MetadataRecord(),
                    CompareTo = new MetadataRecord(),
                    Test = true,
                },
                new {
                    Source = new MetadataRecord(),
                    CompareTo = new MetadataRecord { Id = "id01" },
                    Test = false,
                },
                new {
                    Source = new MetadataRecord { Id = "id01" },
                    CompareTo = new MetadataRecord { Id = "id01" },
                    Test = true,
                },
            };

            foreach (var test in variations)
            {
                (test.Source == test.CompareTo).Should().Be(test.Test);
            }
        }

        [Fact]
        public void GivenPropertiesTests_AllShouldPass()
        {
            var variations = new[]
            {
                new {
                    Source = new MetadataRecord(),
                    CompareTo = new MetadataRecord { Properties = new [] { new KeyValue("property1", "value1") } },
                    Test = false,
                },
                new {
                    Source = new MetadataRecord { Properties = new [] { new KeyValue("property1", "value1") } },
                    CompareTo = new MetadataRecord { Properties = new [] { new KeyValue("property1", "value1") } },
                    Test = true,
                },
                new {
                    Source = new MetadataRecord { Properties = new [] { new KeyValue("property1", "value1") } },
                    CompareTo = new MetadataRecord { Properties = new [] { new KeyValue("property1", "value1"), new KeyValue("property2", "value2") } },
                    Test = false,
                },
                new {
                    Source = new MetadataRecord { Properties = new [] { new KeyValue("property1", "value1"), new KeyValue("property2", "value2") } },
                    CompareTo = new MetadataRecord { Properties = new [] { new KeyValue("property1", "value1"), new KeyValue("property2", "value2") } },
                    Test = true,
                },
            };

            foreach (var test in variations)
            {
                (test.Source == test.CompareTo).Should().Be(test.Test);
            }
        }
    }
}
