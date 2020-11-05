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
        private readonly Regex addRegex = new Regex(@"^(/add)");
        private readonly Regex cartRegex = new Regex(@"^(/cart)");
        private readonly Regex removeRegex = new Regex(@"^(/remove)");
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

                    if (cartRegex.IsMatch(input))
                    {
                        var items = await _pullAndBearService.GetTrackedItemsAsync();
                        var userItems = items.Where(x => x.UserId == message.From.Id).Select(x => $@"
*{x.Name}*
Current: {x.Price} {x.PriceCurrency}
[View on site]({x.Url})
");
                        foreach (var item in userItems)
                        {
                            try
                            {
                                await _botService.Client.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    replyToMessageId: message.MessageId,
                                    parseMode: ParseMode.MarkdownV2,
                                    disableWebPagePreview: false,
                                    text: $@"{item}"
                                );
                            }
                            catch (Exception ex) { }
                        }
                    }

                    if (removeRegex.IsMatch(input))
                    {
                        var itemMessage = message.ReplyToMessage;
                        if (itemMessage == null)
                        {
                            try
                            {
                                await _botService.Client.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    replyToMessageId: message.MessageId,
                                    text: $@"Reply to message with item you want to remove from cart."
                                );
                            }
                            catch (Exception ex) { }
                            return;
                        }
                        try
                        {
                            await _pullAndBearService.RemoveItemAsync(itemMessage);
                        } catch (Exception e)
                        {
                            try
                            {
                                await _botService.Client.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    replyToMessageId: message.MessageId,
                                    parseMode: ParseMode.MarkdownV2,
                                    disableWebPagePreview: false,
                                    text: $@"{e.Message}"
                                );
                            }
                            catch (Exception ex) { }
                        }
                        try
                        {
                            await _botService.Client.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                replyToMessageId: message.MessageId,
                                text: "Item was removed from cart."
                            );
                        }
                        catch (Exception ex) { }
                    }
                }
            }
        }
    }
}
