using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public interface ITrackingRepository
    {
        Task<int> AddNewItemAsync(Item item);

        Task<List<Item>> GetItemsAsync();

        Task UpdateInfoOfItemAsync(int id, string status, int? price, string priceCurrency, string name, string image);

        Task<bool> IsTracked(string url, int userId);
    }
}
