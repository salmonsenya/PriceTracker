using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PriceTracker.Clients;
using PriceTracker.Services;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PriceTracker.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        private readonly IUpdateService _updateService;
        private readonly IPullAndBearService _asosService;
        private readonly IPullAndBearClient _asosClient;

        public TestController(IUpdateService updateService, IPullAndBearService asosService, IPullAndBearClient asosClient)
        {
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            _asosService = asosService ?? throw new ArgumentNullException(nameof(asosService));
            _asosClient = asosClient ?? throw new ArgumentNullException(nameof(asosClient));
        }

        /*[HttpPost]
        [Route("try")]
        public async Task<IActionResult> Try([FromBody] object body)
        {
            var update = JsonConvert.DeserializeObject<Telegram.Bot.Types.Update>(body.ToString());
            if (update == null) return Ok();
            await ReplyAsync(update);
            return Ok();
        }*/

        /*public async Task ReplyAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
                return;

            var message = update.Message;
            if (message.Type == MessageType.Text)
            {
                var input = message.Text;
                if (input != null)
                {
                    var addRegex = new Regex(@"^(/add )");
                    if (addRegex.IsMatch(input))
                    {
                        await AddNewItemAsync(message);
                    }
                }
            }
        }*/

        /*public async Task AddNewItemAsync(Message message)
        {
            var input = message.Text;
            var url = Regex.Match(input, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?").Value;
            var result = await _asosClient.GetItemInfoAsync(url);
            var s = result;
        }*/
    }
}
