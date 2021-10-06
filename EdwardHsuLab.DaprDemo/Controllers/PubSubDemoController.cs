using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("訂閱Topic資訊")]
        public async Task<IActionResult> Subscribe()
        {
            // Programmatically subscribe
            // https://docs.dapr.io/developing-applications/building-blocks/pubsub/howto-publish-subscribe/#programmatic-subscriptions
            return Json(
                new[]
                {
                    new {
                        pubsubname = "kafka-pubsub",
                        topic      = "test1",
                        route      = "/api/PubSubDemo/event"
                    }
                });
        }

        [HttpGet("fire-event")]
        [Description("發送Event")]
        public async Task<IActionResult> FireEvent(string msg)
        {
            await _client.PublishEventAsync(
                "kafka-pubsub", "test1", new
                {
                    msg = msg
                });

            return Ok();
        }

        [HttpPost("event")]
        [Description("Event接收")] 
        public async Task<IActionResult> ReceivedCloudEvent(CloudEvent e)
        {
            await _client.SaveStateAsync("redis-state", "recEvent", e.Data);
            return Ok();
        }

        [HttpGet("received")]
        [Description("取得最後收到的Event內容")]
        public async Task<IActionResult> GetReceivedData()
        {
            return Json(await _client.GetStateAsync<object>("redis-state", "recEvent"));
        }

        
    }
}
