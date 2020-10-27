using Microsoft.AspNetCore.Mvc;

namespace PriceTracker.Controllers
{
    [Route("home")]
    public class HomeController : Controller { 

        [HttpGet]
        [Route("health")]
        public IActionResult Health()
        {
            return Ok("PriceTracker is active.");
        }

        [HttpGet]
        [Route("check")]
        public IActionResult Check()
        {
            return Ok("PriceTracker is active.");
        }
    }
}
