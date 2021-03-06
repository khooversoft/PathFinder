﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinderWeb.Server.Services;

namespace PathFinderWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private readonly LinkService _linkService;

        public LinkController(LinkService linkService)
        {
            _linkService = linkService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            LinkRecord? record = await _linkService.Get(id);
            if (record == null) return NotFound();

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LinkRecord record)
        {
            record.Prepare();

            await _linkService.Set(record);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _linkService.Delete(id);
            return Ok();
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] QueryParameters listParameters)
        {
            BatchSet<LinkRecord> list = await _linkService.List(listParameters);

            var result = new BatchSet<LinkRecord>
            {
                QueryParameters = listParameters,
                NextIndex = listParameters.Index + listParameters.Count,
                Records = list.Records.ToArray(),
            };

            return Ok(result);
        }
    }
}
