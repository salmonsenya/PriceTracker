using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PriceTracker.Services;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PriceTracker.Controllers
{
    [Route("bot")]
    public class UpdateController : Controller
    {
        private readonly IUpdateService _updateService;

        public UpdateController(IUpdateService updateService)
        {
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] object body)
        {
            Update update;
            try
            {
                update = JsonConvert.DeserializeObject<Update>(body.ToString());
            }
            catch (Exception ex)
            {
                return Ok($"Update could not be parsed. Smth went wrong: {ex.Message}");
            }
            if (update == null) return Ok("Update is null.");
            await _updateService.ReplyAsync(update);
            return Ok("Request processed.");
        }

        [HttpGet]
        [Route("check")]
        public IActionResult Check()
        {
            return Ok("PriceTracker is active.");
        }
    }
}
