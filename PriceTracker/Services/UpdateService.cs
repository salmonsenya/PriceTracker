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
        private readonly IPullAndBearService _pullAndBearService;
        private readonly IBotService _botService;
        private readonly ITrackingRepository _trackingRepository;
        private const string urlTemplate = @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
        private const string ADD = "add";
        private const string CART = "cart";
        private const string REMOVE = "remove";
        private const string START = "start";
        private const string HELP = "help";
        private const string HELP_TEXT = @"
/start - send start message
/help - show help
/add {link_to_item} - add link to the item you want to track
add - add link to the item you want to track in the next message to bot
/cart - see your tracked items
cart - see your tracked items";

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

                if (queryData.Equals(REMOVE))
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
                        var isWaitingForAdd = false;
                        var isUserStatusExists = _trackingRepository.IsUserStatusExists(message.From.Id);
                        if (!isUserStatusExists)
                        {
                            await _trackingRepository.AddUserStatusAsync(message.From.Id);
                            isWaitingForAdd = false;
                        } else
                        {
                            isWaitingForAdd = await _trackingRepository.IsWaitingForAddAsync(message.From.Id);
                        }

                        if (input.Equals(ADD))
                        {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, true);
                            await _botService.SendMessage(
                                message.Chat.Id,
                                message.MessageId,
                                "Insert a link of item you want to add.");
                        }
                        else if (addRegex.IsMatch(input))
                        {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                            await _pullAndBearService.AddNewItemAsync(message);
                        }

                        else if (input.Equals($"/{CART}") || input.Equals(CART))
                        {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
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

                            else if (input.Equals($"/{START}"))
                            {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                            await _botService.SendMessage(
                                            message.Chat.Id,
                                            $"{HELP_TEXT}");
                            }

                            else if (input.Equals($"/{HELP}"))
                            {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                            await _botService.SendMessage(
                                            message.Chat.Id,
                                            $"{HELP_TEXT}");
                            }

                        else if (isWaitingForAdd)
                        {
                            var url = Regex.Match(input, urlTemplate).Value;
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
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                            }
                        }
                    }
                }
            }
        }
    }
}
