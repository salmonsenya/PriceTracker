using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PriceTracker.Services;
using System;

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
        public IActionResult Items()
        {
            var items = _pullAndBearService.GetTrackedItems();
            return Ok(JsonConvert.SerializeObject(items, Formatting.Indented));
        }
    }
}
