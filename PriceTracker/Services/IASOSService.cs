using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PriceTracker.Services
{
    public interface IASOSService
    {
        Task AddNewItemAsync(Message message);
    }
}
