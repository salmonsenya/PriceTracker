using System.Threading.Tasks;
using Telegram.Bot;

namespace PriceTracker.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }

        Task SendMessage(long chatId, string text);

        Task SendMessage(long chatId, int replyToMessageId, string text);

        Task SendMessageMarkdownV2(long chatId, int replyToMessageId, string text);

        Task SendMessageMarkdownV2(long chatId, string text);

        Task SendMessageButtonMarkdownV2(long chatId, int replyToMessageId, string text);
    }
}
