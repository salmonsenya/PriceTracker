using PriceTracker.Repositories;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using PriceTracker.Consts;
using PriceTracker.Helpers;

namespace PriceTracker.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IShopService _shopService;
        private readonly IBotService _botService;
        private readonly ITrackingRepository _trackingRepository;
        private readonly ITextConverter _textConverter;

        private readonly Regex addRegex = new Regex($@"^(/{Commands.ADD})");
        private readonly Regex addRegexBot = new Regex($@"^(/{Commands.ADD}@{Common.BOT_NAME})");

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

        private const string EMPTY_CART = "Your cart is empty.";
        private const string NEED_CORRECT_LINK_EXCEPTION = "Insert a correct link.";
        private const string NEED_LINK_EXCEPTION = "Insert a link of item you want to add.";
        private const string REMOVE_EXCEPTION = "Item could not be removed from cart.";
        private const string REMOVED = "Item was removed from cart.";

        public UpdateService(IShopService shopService, IBotService botService, ITrackingRepository trackingRepository, ITextConverter textConverter)
        {
            _shopService = shopService ?? throw new ArgumentNullException(nameof(shopService));
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
            _textConverter = textConverter ?? throw new ArgumentNullException(nameof(textConverter));
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
                        await _shopService.RemoveItemAsync(message);
                        await _botService.SendMessageAsync(
                            message.Chat.Id,
                            message.MessageId,
                            REMOVED);
                    }
                    catch (Exception ex)
                    {
                        await _botService.SendMessageMarkdownV2Async(
                                message.Chat.Id,
                                message.MessageId,
                                $"{REMOVE_EXCEPTION} {ex.Message}");
                    }
                }

            }
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                if (message.Type == MessageType.Text)
                {
                    var input = message.Text;

                    if (input != null) {
                        try {
                            var isWaitingForAdd = false;
                            var isUserStatusExists = _trackingRepository.IsUserStatusExists(message.From.Id);
                            if (isUserStatusExists)
                                isWaitingForAdd = await _trackingRepository.IsWaitingForAddAsync(message.From.Id);
                            else
                                await _trackingRepository.AddUserStatusAsync(message.From.Id);

                            if (input.Equals(Commands.CANCEL))
                            {
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                return;
                            }
                            else if (input.Equals(Commands.ADD))
                            {
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, true);
                                throw new Exception(NEED_LINK_EXCEPTION);
                            }
                            else if (addRegex.IsMatch(input) || addRegexBot.IsMatch(input))
                            {
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                await _shopService.AddNewItemAsync(message);
                            }
                            else if (new List<string>() { $"/{Commands.CART}", Commands.CART, $"/{Commands.CART}@{Common.BOT_NAME}" }.Contains(input))
                            {
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                var items = await _shopService.GetTrackedItemsAsync(message.From.Id);
                                if (items.Count == 0)
                                {
                                    await _botService.SendMessageAsync(
                                        message.Chat.Id,
                                        message.MessageId,
                                        EMPTY_CART);
                                    return;
                                }
                                var textItems = _textConverter.ToStrings(items);
                                foreach (var item in textItems)
                                    await _botService.SendMessageButtonMarkdownV2Async(
                                        message.Chat.Id,
                                        message.MessageId,
                                        $@"{item}");
                            }
                            else if (input.Equals($"/{Commands.START}") || input.Equals($"/{Commands.START}@{Common.BOT_NAME}"))
                            {
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                await _botService.SendMessageAsync(
                                                message.Chat.Id,
                                                $"{HELP_TEXT}");
                            }
                            else if (input.Equals($"/{Commands.HELP}") || input.Equals($"{Commands.HELP}") || input.Equals($"/{Commands.HELP}@{Common.BOT_NAME}"))
                            {
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                await _botService.SendMessageAsync(
                                                message.Chat.Id,
                                                $"{HELP_TEXT}");
                            }
                            else if (isWaitingForAdd)
                            {
                                var url = Regex.Match(input, Common.UrlTemplate).Value;
                                if (string.IsNullOrEmpty(url))
                                    throw new Exception(NEED_CORRECT_LINK_EXCEPTION);

                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                var newItem = await _shopService.AddNewItemAsync(message);
                                await _botService.SendMessageButtonMarkdownV2Async(
                                    message.Chat.Id,
                                    message.MessageId,
                                    _textConverter.ToString(newItem));
                            }
                        } catch (Exception ex)
                        {
                            await _botService.SendMessageAsync(
                                        message.Chat.Id,
                                        message.MessageId,
                                        ex.Message);
                        }
                    }
                }
            }
        }
    }
}
