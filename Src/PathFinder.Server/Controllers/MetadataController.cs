﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathFinder.sdk.Host.PathServices;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;

namespace PathFinder.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataPathService _metadataPathService;

        public MetadataController(IMetadataPathService metadataService)
        {
            _metadataPathService = metadataService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            MetadataRecord? record = await _metadataPathService.Get(id);
            if (record == null) return NotFound();

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MetadataRecord record)
        {
            record.Prepare();

            await _metadataPathService.Set(record);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _metadataPathService.Delete(id);
            return Ok();
        }

        [HttpGet("list/{index?}/{count?}")]
        public async Task<IActionResult> List([FromQuery] QueryParameters listParameters)
        {
            IReadOnlyList<MetadataRecord> list = await _metadataPathService.List(QueryParameters.Default);
            int index = listParameters.Index + listParameters.Count;

            var result = new BatchSet<MetadataRecord>
            {
                ContinuationUrl = $"api/link/list?{listParameters.WithIndex(index).ToQuery()}",
                Records = list.ToArray(),
            };

            return Ok(result);
        }
    }
}
