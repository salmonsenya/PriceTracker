using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PriceTracker.Services
{
    public class UpdateService : IUpdateService
    {

        private readonly IASOSService _asosService;

        public UpdateService(IASOSService asosService)
        {
            _asosService = asosService ?? throw new ArgumentNullException(nameof(asosService));
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
                        await _asosService.AddNewItemAsync(message);
                    }
                }
            }
        }
    }
}
