using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public interface ITrackingRepository
    {
        Task<bool> IsWaitingForAddAsync(int userId);

        Task AddUserStatusAsync(int userId);

        List<UserStatus> GetUserStatuses();

        bool IsUserStatusExists(int userId);

        Task SetWaitingForAddAsync(int userId, bool newValue);

        Task<int> AddNewItemAsync(Item item);

        Task<List<Item>> GetItemsAsync();

        Task UpdateInfoOfItemAsync(int id, ItemOnline itemOnline);

        Task<bool> IsTrackedAsync(string url, int userId);

        Task RemoveItemAsync(string url);
    }
}
