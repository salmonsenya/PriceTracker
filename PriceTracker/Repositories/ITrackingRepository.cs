using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public interface ITrackingRepository
    {
        Task<bool> IsWaitingForAddAsync(int userId);

        Task<bool> AddUserStatusAsync(int userId);

        Task<List<UserStatus>> GetUserStatusesAsync();

        Task<bool> IsUserStatusExistsAsync(int userId);

        Task SetWaitingForAddAsync(int userId, bool newValue);

        Task<int> AddNewItemAsync(Item item);

        Task<List<Item>> GetItemsAsync();

        Task UpdateInfoOfItemAsync(Item itemUpdated);

        Task<bool> IsTrackedAsync(string url, int userId);

        Task RemoveItemAsync(string url);
    }
}
