using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathFinderWeb.Server.Services;
using PathFinderWeb.Shared;

namespace PathFinderWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly ContentService _contentService;

        public ConfigController(ContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet]
        public Configuration Get() => new Configuration();

        [HttpGet("doc/{docId}")]
        public DocItem GetDoc(string docId)
        {
            return new DocItem
            {
                DocId = docId,
                Html = _contentService.GetDocHtml(docId),
            };
        }
    }
}
