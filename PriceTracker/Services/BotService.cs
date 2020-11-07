using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PriceTracker.Services
{
    public class BotService : IBotService
    {
        private readonly BotOptions _botOptions;
        private readonly string botToken;
        public TelegramBotClient Client { get;  }

        public BotService(IOptions<BotOptions> botOptions)
        {
            _botOptions = botOptions?.Value ?? throw new ArgumentNullException(nameof(botOptions));
            botToken = _botOptions.BotToken ?? throw new ArgumentNullException(nameof(BotOptions.BotToken));
            Client = new TelegramBotClient(botToken);
        }

        public async Task SendMessage(long chatId, int replyToMessageId, string text)
        {
            try
            {
                await Client.SendTextMessageAsync(
                    chatId: chatId,
                    replyToMessageId: replyToMessageId,
                    text: text);
            }
            catch (Exception ex) { }
        }

        public async Task SendMessageMarkdownV2(long chatId, int replyToMessageId, string text)
        {
            try
            {
                await Client.SendTextMessageAsync(
                    chatId: chatId,
                    replyToMessageId: replyToMessageId,
                    parseMode: ParseMode.MarkdownV2,
                    disableWebPagePreview: false,
                    text: text);
            }
            catch (Exception ex) { }
        }

        public async Task SendMessageMarkdownV2(long chatId, string text)
        {
            try
            {
                await Client.SendTextMessageAsync(
                    chatId: chatId,
                    parseMode: ParseMode.MarkdownV2,
                    disableWebPagePreview: false,
                    text: text);
            }
            catch (Exception ex) { }
        }

        public async Task SendMessageButtonMarkdownV2(long chatId, int replyToMessageId, string text)
        {
            var inlineKeyboardButton = new InlineKeyboardButton
            {
                Text = "remove",
                CallbackData = "remove"
            };
            var keyboard = new InlineKeyboardMarkup(inlineKeyboardButton);
            try
            {
                await Client.SendTextMessageAsync(
                    chatId: chatId,
                    replyToMessageId: replyToMessageId,
                    parseMode: ParseMode.MarkdownV2,
                    disableWebPagePreview: false,
                    replyMarkup: keyboard,
                    text: text);
            }
            catch (Exception ex) { }
        }
    }
}
