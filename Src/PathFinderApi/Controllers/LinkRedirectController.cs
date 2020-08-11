using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathFinder.sdk.Actors;
using PathFinder.sdk.Host.PathServices;
using PathFinder.sdk.Records;

namespace PathFinderApi.Controllers
{
    [Route("link")]
    [ApiController]
    public class LinkRedirectController : ControllerBase
    {
        private readonly ILinkPathService _linkPathService;

        public LinkRedirectController(ILinkPathService linkRecordActor)
        {
            _linkPathService = linkRecordActor;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRedirect(string id)
        {
            LinkRecord? linkRecord = await _linkPathService.Get(id);
            if (linkRecord == null) return NotFound();

            return Redirect(linkRecord.RedirectUrl);
        }
    }
}
