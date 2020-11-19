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
        public void GivenIdTests_AllShouldPass()
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
            };

            foreach (var test in variations)
            {
                (test.Source == test.CompareTo).Should().Be(test.Test);
            }
        }

        [Fact]
        public void GivenRedirectTests_AllShouldPass()
        {
            var variations = new[]
            {
                new {
                    Source = new LinkRecord(),
                    CompareTo = new LinkRecord { RedirectUrl = "redirect01" },
                    Test = false,
                },
                new {
                    Source = new LinkRecord { RedirectUrl = "redirect01" },
                    CompareTo = new LinkRecord { RedirectUrl = "redirect01" },
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
