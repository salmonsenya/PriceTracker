using PriceTracker.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using PriceTracker.Consts;

namespace PriceTracker.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly Regex addRegex = new Regex($@"^(/{Commands.ADD})");
        private readonly Regex addRegexBot = new Regex($@"^(/{Commands.ADD}@{Common.BOT_NAME})");
        private readonly IShopService _pullAndBearService;
        private readonly IBotService _botService;
        private readonly ITrackingRepository _trackingRepository;
        
        private readonly string HELP_TEXT = $@"
PULL&BEAR and Bershka items are only available for tracking at this moment.

/{Commands.START} - send start message
/{Commands.HELP} - show help
{Commands.HELP} - show help
/{Commands.ADD} link_to_item - add link to the item you want to track
{Commands.ADD} - add link to the item you want to track in the next message to bot
/{Commands.CART} - see your tracked items
{Commands.CART} - see your tracked items
{Commands.CANCEL} - cancel adding of item";

        public UpdateService(IShopService asosService, IBotService botService, ITrackingRepository trackingRepository)
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

                if (queryData.Equals(Commands.REMOVE))
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

                        if (input.Equals(Commands.CANCEL))
                        {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                            return;
                        }
                        if (input.Equals(Commands.ADD))
                        {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, true);
                            await _botService.SendMessage(
                                message.Chat.Id,
                                message.MessageId,
                                "Insert a link of item you want to add.");
                        }
                        else if (addRegex.IsMatch(input) || addRegexBot.IsMatch(input))
                        {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                            await _pullAndBearService.AddNewItemAsync(message);
                        }
                        else if (new List<string>() { $"/{Commands.CART}", Commands.CART, $"/{Commands.CART}@{Common.BOT_NAME}" }.Contains(input))
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

                            else if (input.Equals($"/{Commands.START}") || input.Equals($"/{Commands.START}@{Common.BOT_NAME}"))
                            {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                            await _botService.SendMessage(
                                            message.Chat.Id,
                                            $"{HELP_TEXT}");
                            }

                            else if (input.Equals($"/{Commands.HELP}") || input.Equals($"{Commands.HELP}") || input.Equals($"/{Commands.HELP}@{Common.BOT_NAME}"))
                            {
                            await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                            await _botService.SendMessage(
                                            message.Chat.Id,
                                            $"{HELP_TEXT}");
                            }

                        else if (isWaitingForAdd)
                        {
                            var url = Regex.Match(input, Common.UrlTemplate).Value;
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
