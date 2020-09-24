using Microsoft.AspNetCore.Mvc;

namespace PriceTracker.Controllers
{
    [Route("home")]
    public class HomeController : Controller { 

        [HttpGet]
        [Route("health")]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}
