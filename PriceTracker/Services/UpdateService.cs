using PriceTracker.Repositories;
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
        private readonly ITrackingRepository _trackingRepository;

        public UpdateService(IPullAndBearService asosService, IBotService botService, ITrackingRepository trackingRepository)
        {
            _pullAndBearService = asosService ?? throw new ArgumentNullException(nameof(asosService));
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
        }

        public async Task ReplyAsync(Update update)
        {
            if (update.CallbackQuery != null)
            {
                var message = update.CallbackQuery.Message;
                var queryData = update.CallbackQuery.Data;

                if (queryData.Equals("remove"))
                {
                    try
                    {
                        await _pullAndBearService.RemoveItemAsync(message);
                    }
                    catch (Exception e)
                    {
                        await _botService.SendMessageMarkdownV2(
                                message.Chat.Id,
                                message.MessageId,
                                $@"{e.Message}");
                    }
                    await _botService.SendMessage(
                            message.Chat.Id,
                            message.MessageId,
                            "Item was removed from cart."); 
                }

            }
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                if (message.Type == MessageType.Text)
                {
                    var input = message.Text;

                    if (input != null) {
                        if (!await _trackingRepository.IsWaitingForAdd(message.From.Id))
                        {
                            if (input.Equals("add"))
                            {
                                await _trackingRepository.SetWaitingForAdd(message.From.Id, true);
                                await _botService.SendMessage(
                        message.Chat.Id,
                        message.MessageId,
                        "Insert a link of item you want to add.");
                            }
                            if (addRegex.IsMatch(input))
                            {
                                await _pullAndBearService.AddNewItemAsync(message);
                            }

                            if (cartRegex.IsMatch(input) || input.Equals("cart"))
                            {
                                var items = await _pullAndBearService.GetTrackedItemsAsync();
                                if (items?.Count == 0)
                                {
                                    await _botService.SendMessage(
                                            message.Chat.Id,
                                            message.MessageId,
                                            "Your cart is empty.");
                                }
                                var userItems = items.Where(x => x.UserId == message.From.Id).Select(x => $@"
*{x.Name}*
Current: {x.Price} {x.PriceCurrency}
[View on site]({x.Url})
");
                                foreach (var item in userItems)
                                {
                                    await _botService.SendMessageButtonMarkdownV2(
                                            message.Chat.Id,
                                            message.MessageId,
                                            $@"{item}");
                                }
                            }

                            if (input.Equals("/start"))
                            {
                                await _botService.SendMessageMarkdownV2(
                                            message.Chat.Id,
                                            "Start");
                            }

                            if (input.Equals("/help"))
                            {
                                await _botService.SendMessageMarkdownV2(
                                            message.Chat.Id,
                                            "Help");
                            }

                            if (input.Equals("remove"))
                            {
                                await _botService.SendMessage(
                                            message.Chat.Id,
                                            message.MessageId,
                                            "Reply with /remove to message with item you want to remove from cart or use inline button remove after /cart command.");
                            }
                            if (removeRegex.IsMatch(input))
                            {
                                var itemMessage = message.ReplyToMessage;
                                if (itemMessage == null)
                                {
                                    await _botService.SendMessage(
                                            message.Chat.Id,
                                            message.MessageId,
                                            "Reply with /remove to message with item you want to remove from cart or use inline button remove after /cart command.");
                                    return;
                                }
                                try
                                {
                                    await _pullAndBearService.RemoveItemAsync(itemMessage);
                                }
                                catch (Exception e)
                                {
                                    await _botService.SendMessageMarkdownV2(
                                            message.Chat.Id,
                                            message.MessageId,
                                            $@"{e.Message}");
                                }
                                await _botService.SendMessage(
                                        message.Chat.Id,
                                        message.MessageId,
                                        "Item was removed from cart."
                                    );
                            }
                        }
                        else
                        {
                            var url = Regex.Match(input, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?").Value;
                            if (string.IsNullOrEmpty(url))
                            {
                                await _botService.SendMessage(
                                    message.Chat.Id,
                                    message.MessageId,
                                    "Insert a correct link.");
                            }
                            else
                            {
                                await _pullAndBearService.AddNewItemAsync(message);
                                await _trackingRepository.SetWaitingForAdd(message.From.Id, false);
                            }
                        }
                    }
                }
            }
        }
    }
}
