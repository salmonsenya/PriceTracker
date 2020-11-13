using System.Threading.Tasks;
using Telegram.Bot;

namespace PriceTracker.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }

        Task SendMessageAsync(long chatId, string text);

        Task SendMessageAsync(long chatId, int replyToMessageId, string text);

        Task SendMessageMarkdownV2Async(long chatId, int replyToMessageId, string text);

        Task SendMessageMarkdownV2Async(long chatId, string text);

        Task SendMessageButtonMarkdownV2Async(long chatId, int replyToMessageId, string text);

        Task SendMessageButtonMarkdownV2Async(long chatId, string text);
    }
}
