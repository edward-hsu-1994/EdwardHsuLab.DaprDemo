using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Dapr.Client;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;

namespace EdwardHsuLab.DaprDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvokeDemoController : ControllerBase
    {
        private readonly ILogger<PubSubDemoController> _logger;
        private readonly DaprClient _client;

        public InvokeDemoController(
            ILogger<PubSubDemoController> logger,
            DaprClient client
        )
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet("retryStatusStore")]
        [Description("測試用，這是要讓retryInvoke使用的API，對同一個key存取第三次時才會正常")]
        public async Task<int> RetryStatusStoreTest(string name = "default")
        {
            var retryTime = await _client.GetStateAsync<int>("redis-state", name);


            retryTime++;
            

            if (retryTime >= 3)
            {
                return retryTime;
            }

            await _client.SaveStateAsync("redis-state", name, retryTime);

            // force disconnection
            this.HttpContext.Abort();
            
            return retryTime;
        }

        [HttpGet("retryInvoke")]
        [Description("調用其他服務的API(RETRY測試)")]
        public async Task<int> RetryInvokeTest(string name = "default")
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("name", name);

            // DAPR 再發生服務中斷或者網路問題(憑證之類的)會自動重試
            // https://docs.dapr.io/developing-applications/building-blocks/service-invocation/service-invocation-overview/#retries
            return await _client.InvokeMethodAsync<int>(
                System.Net.Http.HttpMethod.Get, 
                "daprdemoapp",
                "api/invokeDemo/retryStatusStore?" + query.ToString());
        }
    }
}
