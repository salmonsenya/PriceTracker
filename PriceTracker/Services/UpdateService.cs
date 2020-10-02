using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PriceTracker.Services
{
    public class UpdateService : IUpdateService
    {

        private readonly IPullAndBearService _pullAndBearService;

        public UpdateService(IPullAndBearService asosService)
        {
            _pullAndBearService = asosService ?? throw new ArgumentNullException(nameof(asosService));
        }

        public async Task ReplyAsync(Update update)
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
                        await _pullAndBearService.AddNewItemAsync(message);
                    }
                }
            }
        }
    }
}
