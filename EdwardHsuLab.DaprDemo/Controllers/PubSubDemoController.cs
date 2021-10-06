using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using CloudNative.CloudEvents;

namespace EdwardHsuLab.DaprDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PubSubDemoController : Controller 
    {
        private readonly ILogger<PubSubDemoController> _logger;
        private readonly DaprClient _client;

        public PubSubDemoController(
            ILogger<PubSubDemoController> logger,
            DaprClient client
        )
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet("/dapr/subscribe")]
        public async Task<IActionResult> Subscribe()
        { 
            return Json(
                new[]
                {
                    new {
                        pubsubname = "kafka-pubsub",
                        topic      = "test1",
                        route      = "/api/PubSubDemo/received"
                    }
                });
        }

        [HttpPost("received")]
        public async Task<IActionResult> ReceivedCloudEvent(CloudEvent e)
        {
            return Ok();
        }

        [HttpGet("fire-event")]
        public async Task<IActionResult> FireEvent(string msg)
        {
            await _client.PublishEventAsync(
                "kafka-pubsub", "test1", new
                {
                    msg = msg
                });

            return Ok();
        }
    }
}
