using FluentAssertions;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using System;
using System.Collections.Generic;
using Xunit;

namespace PathFinder.sdk.Test
{
    public class LinkRecordEqutityTests
    {
        [Fact]
        public void GivenMultipleTest_AllShouldPass()
        {
            var variations = new[]
            {
                new {
                    Source = new LinkRecord(),
                    CompareTo = new LinkRecord(),
                    Test = true,
                },
                new {
                    Source = new LinkRecord(),
                    CompareTo = new LinkRecord { Id = "id01" },
                    Test = false,
                },
                new {
                    Source = new LinkRecord { Id = "id01" },
                    CompareTo = new LinkRecord { Id = "id01" },
                    Test = true,
                },

                new {
                    Source = new LinkRecord { Id = "id01" },
                    CompareTo = new LinkRecord { Id = "id01", RedirectUrl = "redirect01" },
                    Test = false,
                },
                new {
                    Source = new LinkRecord { Id = "id01", RedirectUrl = "redirect01" },
                    CompareTo = new LinkRecord { Id = "id01", RedirectUrl = "redirect01" },
                    Test = true,
                },

                new {
                    Source = new LinkRecord { Id = "id01", RedirectUrl = "redirect01" },
                    CompareTo = new LinkRecord { Id = "id01", RedirectUrl = "redirect01", Owner = "owner01" },
                    Test = false,
                },
                new {
                    Source = new LinkRecord { Id = "id01", RedirectUrl = "redirect01", Owner = "owner01" },
                    CompareTo = new LinkRecord { Id = "id01", RedirectUrl = "redirect01", Owner = "owner01" },
                    Test = true,
                },

                new {
                    Source = new LinkRecord { Id = "id01", RedirectUrl = "redirect01", Owner = "owner01" },
                    CompareTo = new LinkRecord { Id = "id01", RedirectUrl = "redirect01", Owner = "owner01", Enabled = true },
                    Test = true,
                },
                new {
                    Source = new LinkRecord { Id = "id01", RedirectUrl = "redirect01", Owner = "owner01", Enabled = true },
                    CompareTo = new LinkRecord { Id = "id01", RedirectUrl = "redirect01", Owner = "owner01", Enabled = true },
                    Test = true,
                },

                new {
                    Source = new LinkRecord { Id = "id01", RedirectUrl = "redirect01", Owner = "owner01", Enabled = true },
                    CompareTo = new LinkRecord
                    {
                        Id = "id01",
                        RedirectUrl = "redirect01",
                        Owner = "owner01",
                        Enabled = true,
                        Tags = new List<Tag>
                        {
                            new Tag("key1", "value1"),
                        }
                    },
                    Test = false,
                },
                new {
                    Source = new LinkRecord
                    {
                        Id = "id01",
                        RedirectUrl = "redirect01",
                        Owner = "owner01",
                        Enabled = true,
                        Tags = new List<Tag>
                        {
                            new Tag("key1", "value1"),
                        }
                    },
                    CompareTo = new LinkRecord
                    {
                        Id = "id01",
                        RedirectUrl = "redirect01",
                        Owner = "owner01",
                        Enabled = true,
                        Tags = new List<Tag>
                        {
                            new Tag("key1", "value1"),
                        }
                    },
                    Test = true,
                },
                new {
                    Source = new LinkRecord
                    {
                        Id = "id01",
                        RedirectUrl = "redirect01",
                        Owner = "owner01",
                        Enabled = true,
                        Tags = new List<Tag>
                        {
                            new Tag("key1", "value1"),
                        }
                    },
                    CompareTo = new LinkRecord
                    {
                        Id = "id01",
                        RedirectUrl = "redirect01",
                        Owner = "owner01",
                        Enabled = true,
                        Tags = new List<Tag>
                        {
                            new Tag("key1", "value3"),
                        }
                    },
                    Test = false,
                },
            };

            foreach (var test in variations)
            {
                (test.Source == test.CompareTo).Should().Be(test.Test);
            }
        }
    }
}
