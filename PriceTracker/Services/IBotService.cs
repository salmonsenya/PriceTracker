using System.Threading.Tasks;
using Telegram.Bot;

namespace PriceTracker.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }

        Task SendReplyMessage(long chatId, int replyToMessageId, string text);

        Task SendReplyMessageMarkdownV2(long chatId, int replyToMessageId, string text);

        Task SendMessageMarkdownV2(long chatId, string text);
    }
}
