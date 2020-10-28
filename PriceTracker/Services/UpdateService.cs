using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PriceTracker.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly Regex addRegex = new Regex(@"^(/add )");
        private readonly Regex itemsRegex = new Regex(@"^(/items)");
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
                    
                    if (addRegex.IsMatch(input))
                    {
                        await _pullAndBearService.AddNewItemAsync(message);
                    }

                    if (itemsRegex.IsMatch(input))
                    {
                        var items = _pullAndBearService.GetTrackedItems().Select(x => $@"
*{x.Name}*
[Смотреть на сайте]({x.Url})

"
).ToList();
                        var itemsText = string.Join("", items);
                        try
                        {
                            await _botService.Client.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                replyToMessageId: message.MessageId,
                                parseMode: ParseMode.MarkdownV2,
                                disableWebPagePreview: true,
                                text: $@"
{itemsText}"
                            );
                        }
                        catch (Exception ex) { }
                    }
                }
            }
        }
    }
}
