using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public interface ITrackingRepository
    {
        Task<bool> IsWaitingForAdd(int userId);

        Task AddUserStatus(int userId);

        Task SetWaitingForAdd(int userId, bool newValue);

        Task<int> AddNewItemAsync(Item item);

        Task<List<Item>> GetItemsAsync();

        Task UpdateInfoOfItemAsync(int id, ItemOnline itemOnline);

        Task<bool> IsTracked(string url, int userId);

        Task RemoveItem(string url);
    }
}
