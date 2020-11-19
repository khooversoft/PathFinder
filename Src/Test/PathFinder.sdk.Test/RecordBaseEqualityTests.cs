using FluentAssertions;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PathFinder.sdk.Test
{
    public class RecordBaseEqualityTests
    {
        [Fact]
        public void GivenRecordTypeTests_AllShouldPass()
        {
            var variations = new[]
            {
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase(),
                    Test = true,
                },
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase { RecordType = "RecordType01" },
                    Test = false,
                },
                new {
                    Source = new RecordBase { RecordType = "RecordType01" },
                    CompareTo = new RecordBase { RecordType = "RecordType01" },
                    Test = true,
                },
            };

            foreach (var test in variations)
            {
                (test.Source == test.CompareTo).Should().Be(test.Test);
            }
        }

        [Fact]
        public void GivenEnable_AllShouldPass()
        {
            var variations = new[]
            {
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase(),
                    Test = true,
                },
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase { Enabled = false },
                    Test = false,
                },
                new {
                    Source = new RecordBase { Enabled = false },
                    CompareTo = new RecordBase { Enabled = false },
                    Test = true,
                },
            };

            foreach (var test in variations)
            {
                (test.Source == test.CompareTo).Should().Be(test.Test);
            }
        }

        [Fact]
        public void GivenOwnerTests_AllShouldPass()
        {
            var variations = new[]
            {
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase(),
                    Test = true,
                },
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase { Owner = "owner01" },
                    Test = false,
                },
                new {
                    Source = new RecordBase { Owner = "owner01" },
                    CompareTo = new RecordBase { Owner = "owner01" },
                    Test = true,
                },
            };

            foreach (var test in variations)
            {
                (test.Source == test.CompareTo).Should().Be(test.Test);
            }
        }

        [Fact]
        public void GivenNoteTests_AllShouldPass()
        {
            var variations = new[]
            {
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase(),
                    Test = true,
                },
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase { Note = "Note01" },
                    Test = false,
                },
                new {
                    Source = new RecordBase { Note = "Note01" },
                    CompareTo = new RecordBase { Note = "Note01" },
                    Test = true,
                },
            };

            foreach (var test in variations)
            {
                (test.Source == test.CompareTo).Should().Be(test.Test);
            }
        }

        [Fact]
        public void GivenTags_AllShouldPass()
        {
            var variations = new[]
            {
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase(),
                    Test = true,
                },
                new {
                    Source = new RecordBase(),
                    CompareTo = new RecordBase { Tags = new [] { new KeyValue("key1", "value1") } },
                    Test = false,
                },
                new {
                    Source = new RecordBase { Tags = new [] { new KeyValue("key1", "value1") } },
                    CompareTo = new RecordBase { Tags = new [] { new KeyValue("key1", "value1") } },
                    Test = true,
                },
                new {
                    Source = new RecordBase { Tags = new [] { new KeyValue("key1", "value1") } },
                    CompareTo = new RecordBase { Tags = new [] { new KeyValue("key1", "value1"), new KeyValue("key2", "value2") } },
                    Test = false,
                },
                new {
                    Source = new RecordBase { Tags = new [] { new KeyValue("key1", "value1"), new KeyValue("key2", "value2") } },
                    CompareTo = new RecordBase { Tags = new [] { new KeyValue("key1", "value1"), new KeyValue("key2", "value2") } },
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
