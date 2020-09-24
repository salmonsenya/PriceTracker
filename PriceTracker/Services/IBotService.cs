using Telegram.Bot;

namespace PriceTracker.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
