using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PriceTracker.Services
{
    public interface IUpdateService
    {
        Task ReplyAsync(Update update);
    }
}
