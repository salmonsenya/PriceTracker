using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PriceTracker.Services
{
    public interface IPullAndBearService
    {
        Task AddNewItemAsync(Message message);
    }
}
