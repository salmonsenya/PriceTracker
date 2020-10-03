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
        private readonly IBotService _botService;

        public UpdateService(IPullAndBearService asosService, IBotService botService)
        {
            _pullAndBearService = asosService ?? throw new ArgumentNullException(nameof(asosService));
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
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
                        await _botService.Client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            replyToMessageId: message.MessageId,
                            text: "nice, it's message with /add");
                        await _pullAndBearService.AddNewItemAsync(message);
                    }
                }
            }
        }
    }
}
