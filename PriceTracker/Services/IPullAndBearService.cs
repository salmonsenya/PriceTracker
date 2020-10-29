using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PriceTracker.Services
{
    public interface IPullAndBearService
    {
        Task AddNewItemAsync(Message message);

        Task<List<Item>> GetTrackedItemsAsync();
    }
}
