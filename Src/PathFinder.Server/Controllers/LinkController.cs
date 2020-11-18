using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathFinder.sdk.Host.PathServices;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using Toolbox.Extensions;

namespace PathFinder.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private readonly ILinkPathService _linkPathService;

        public LinkController(ILinkPathService linkRecordActor)
        {
            _linkPathService = linkRecordActor;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            LinkRecord? record = await _linkPathService.Get(id);
            if (record == null) return NotFound();

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LinkRecord record)
        {
            record.Prepare();

            await _linkPathService.Set(record);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _linkPathService.Delete(id);
            return Ok();
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] QueryParameters listParameters)
        {
            IReadOnlyList<LinkRecord> list = await _linkPathService.List(listParameters);

            var result = new BatchSet<LinkRecord>
            {
                QueryParameters = listParameters,
                NextIndex = listParameters.Index + listParameters.Count,
                Records = list.ToArray(),
            };

            return Ok(result);
        }
    }
}
