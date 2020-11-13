using PriceTracker.Repositories;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using PriceTracker.Consts;
using PriceTracker.Helpers;
using System.Linq;

namespace PriceTracker.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IShopService _shopService;
        private readonly IBotService _botService;
        private readonly ITrackingRepository _trackingRepository;
        private readonly ITextConverter _textConverter;

        private readonly string HELP_TEXT = $@"
PULL&BEAR and Bershka items are only available for tracking at this moment.

{Commands.SLASH_START} - send start message
{Commands.SLASH_HELP} - show help
{Commands.HELP} - show help
{Commands.SLASH_ADD} link_to_item - add link to the item you want to track
{Commands.ADD} - add link to the item you want to track in the next message to bot
{Commands.SLASH_CART} - see your tracked items
{Commands.CART} - see your tracked items
{Commands.CANCEL} - cancel adding of item";

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
                var input = update.CallbackQuery.Data;

                switch (input)
                {
                    case Commands.REMOVE:
                        try
                        {
                            await _shopService.RemoveItemAsync(message);
                            await _botService.SendMessageAsync(
                                message.Chat.Id,
                                message.MessageId,
                                Exceptions.REMOVED);
                        }
                        catch (Exception ex)
                        {
                            await _botService.SendMessageMarkdownV2Async(
                                    message.Chat.Id,
                                    message.MessageId,
                                    ex.Message);
                        }
                        break;
                    default:
                        break;
                }
            }
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                if (message.Type != MessageType.Text)
                    return;

                var input = message.Text;
                if (input == null)
                    return;

                try
                {
                    var isUserStatusExists = await _trackingRepository.IsUserStatusExistsAsync(message.From.Id);
                    var isWaitingForAdd = isUserStatusExists ? await _trackingRepository.IsWaitingForAddAsync(message.From.Id) :
                         await _trackingRepository.AddUserStatusAsync(message.From.Id);

                    if (input.StartsWith(Commands.SLASH_ADD) || input.StartsWith(Commands.ADD_BOT))
                    {
                        await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                        await _shopService.AddNewItemAsync(message);
                    }
                    else switch (input)
                        {
                            case Commands.ADD:
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, true);
                                throw new Exception(Exceptions.NEED_LINK_EXCEPTION);
                            case Commands.CANCEL:
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                return;
                            case Commands.SLASH_CART:
                            case Commands.CART:
                            case Commands.CART_BOT:
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                var items = await _shopService.GetTrackedItemsAsync(message.From.Id);
                                if (!items.Any()) {
                                    if (message.From.Username != null)
                                        await _botService.SendMessageMarkdownV2Async(
                                            message.Chat.Id,
                                             $@"[@{message.From.Username}](tg://user?id={message.From.Id}) {Exceptions.EMPTY_CART}");
                                    else
                                        await _botService.SendMessageAsync(
                                            message.Chat.Id,
                                            message.MessageId,
                                            Exceptions.EMPTY_CART);
                                    return;
                                }
                                var textItems = _textConverter.ToStrings(items);
                                foreach (var item in textItems) {
                                    if (message.From.Username != null)
                                        await _botService.SendMessageButtonMarkdownV2Async(
                                            message.Chat.Id,
                                            $@"{item}
[@{message.From.Username}](tg://user?id={message.From.Id})");
                                    else
                                        await _botService.SendMessageButtonMarkdownV2Async(
                                            message.Chat.Id,
                                            message.MessageId,
                                            item);
                                }
                                break;
                            case Commands.SLASH_START:
                            case Commands.START_BOT:
                            case Commands.SLASH_HELP:
                            case Commands.HELP:
                            case Commands.HELP_BOT:
                                await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                await _botService.SendMessageAsync(
                                                message.Chat.Id,
                                                $"{HELP_TEXT}");
                                break;
                            default:
                                if (isWaitingForAdd)
                                {
                                    var url = Regex.Match(input, Common.UrlTemplate).Value;
                                    if (string.IsNullOrEmpty(url))
                                        throw new Exception(Exceptions.NEED_CORRECT_LINK_EXCEPTION);

                                    await _trackingRepository.SetWaitingForAddAsync(message.From.Id, false);
                                    var newItem = await _shopService.AddNewItemAsync(message);
                                    await _botService.SendMessageButtonMarkdownV2Async(
                                        message.Chat.Id,
                                        message.MessageId,
                                        _textConverter.ToString(newItem));
                                }
                                break;
                        }
                }
                catch (Exception ex)
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
