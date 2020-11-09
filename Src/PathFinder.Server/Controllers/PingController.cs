﻿using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using PathFinder.sdk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toolbox.Services;

namespace PathFinder.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly ITelemetryMemory _telemetryMemory;

        public PingController(ITelemetryMemory telemetryMemory)
        {
            _telemetryMemory = telemetryMemory;
        }

        /// <summary>
        /// Get the last 100 logs in reverse order
        /// </summary>
        /// <returns>array of strings</returns>
        [OpenApiOperation("Logs", "Return the last 100 internal operation logs")]
        [OpenApiIgnore]
        [HttpGet("Logs")]
        public ActionResult<PingLogs> GetLogs()
        {
            IReadOnlyList<string> logs = _telemetryMemory.GetLoggedMessages();

            var response = new PingLogs
            {
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
                Count = logs.Count,
                Messages = logs
                    .Take(100)
                    .ToList(),
            };

            return Ok(response);
        }

        /// <summary>
        /// Ping to get current state of the ML Service
        ///
        /// Booting - System is booting
        /// Starting - Service is starting
        /// Running - Service is running
        /// Failed - Service has failed to start
        ///
        /// </summary>
        /// <returns>status</returns>
        [OpenApiOperation("Ping", "Returns the state of the server")]
        [HttpGet]
        public ActionResult<PingResponse> Ping()
        {
            return Ok(GetOkResponse());
        }

        /// <summary>
        /// Returns 200 if the server is ready to process requests
        /// </summary>
        /// <returns></returns>
        [OpenApiOperation("Ready", "Returns 200 if the server is ready to process requests")]
        [HttpGet("ready")]
        public ActionResult Ready()
        {
            return Ok(GetOkResponse());
        }

        /// <summary>
        /// Return 200 if the server is running
        /// </summary>
        /// <returns></returns>
        [OpenApiOperation("Running", "Returns 200 if the server is running")]
        [HttpGet("running")]
        public ActionResult Running()
        {
            return Ok(GetOkResponse());
        }

        private PingResponse GetOkResponse() => new PingResponse
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
            Status = "Running",
        };
    }
}