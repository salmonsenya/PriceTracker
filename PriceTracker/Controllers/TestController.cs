using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PriceTracker.Services;
using System;
using System.Threading.Tasks;

namespace PriceTracker.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        private readonly IPullAndBearService _pullAndBearService;

        public TestController(IPullAndBearService pullAndBearService)
        {
            _pullAndBearService = pullAndBearService ?? throw new ArgumentNullException(nameof(pullAndBearService));
        }

        [HttpGet]
        [Route("check")]
        public IActionResult Check()
        {
            return Ok("PriceTracker is active.");
        }

        [HttpGet]
        [Route("items")]
        public async Task<IActionResult> Items()
        {
            var items = await _pullAndBearService.GetTrackedItemsAsync();
            return Ok(JsonConvert.SerializeObject(items, Formatting.Indented));
        }
    }
}
