using System;
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
    public class MetadataController : ControllerBase
    {
        private readonly MetadataService _metadataService;

        public MetadataController(MetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            MetadataRecord? record = await _metadataService.Get(id);
            if (record == null) return NotFound();

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MetadataRecord record)
        {
            record.Prepare();

            await _metadataService.Set(record);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _metadataService.Delete(id);
            return Ok();
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] QueryParameters listParameters)
        {
            BatchSet<MetadataRecord> list = await _metadataService.List(listParameters);

            var result = new BatchSet<MetadataRecord>
            {
                QueryParameters = listParameters,
                NextIndex = listParameters.Index + listParameters.Count,
                Records = list.Records.ToArray(),
            };

            return Ok(result);
        }
    }
}
