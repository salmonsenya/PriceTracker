using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public interface ITrackingRepository
    {
        Task<int> AddNewItemAsync(Item item);

        Task<List<Item>> GetItemsAsync();

        Task UpdateInfoOfItemAsync(int id, ItemOnline itemOnline);

        Task<bool> IsTracked(string url, int userId);

        Task RemoveItem(string url);
    }
}
