using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PriceTracker.Services
{
    public interface IShopService
    {
        Task<Item> AddNewItemAsync(Message message);

        Task<List<Item>> GetTrackedItemsAsync(int userId);

        Task RemoveItemAsync(Message itemMessage);
    }
}
