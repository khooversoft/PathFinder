using FluentAssertions;
using PathFinder.sdk.Records;
using PathFinderApi.Test.Application;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Services;
using Xunit;

namespace PathFinderApi.Test
{
    public class RedirectTests : IClassFixture<TestApplication>
    {
        private readonly TestApplication _testApplication;

        public RedirectTests(TestApplication testApplication)
        {
            _testApplication = testApplication;
        }

        [Fact]
        public async Task GivenLinkRecord_WhenRedirect_ReturnsRedirect()
        {
            TestWebsiteHost host = _testApplication.GetHost();

            const string id = "lnk0001";
            const string redirectUrl = "http://localhost:5003/Document";

            var record = new LinkRecord
            {
                Id = id,
                RedirectUrl = redirectUrl
            };

            await host.PathFinderClient.Link.Set(record);

            HttpResponseMessage response = await host.Client.GetAsync($"link/{record.Id}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers?.Location?.Should().NotBeNull();
            response.Headers!.Location!.ToString().Should().Be(record.RedirectUrl);
        }
    }
}
