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

        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] QueryParameters listParameters)
        {
            IReadOnlyList<LinkRecord> list = await _linkPathService.List(listParameters);
            int index = listParameters.Index + listParameters.Count;

            var result = new BatchSet<LinkRecord>
            {
                ContinuationUrl = $"api/link/list?{listParameters.WithIndex(index).ToQuery()}",
                Records = list.ToArray(),
            };

            return Ok(result);
        }
    }
}
