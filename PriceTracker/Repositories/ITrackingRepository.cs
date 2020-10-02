using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public interface ITrackingRepository
    {
        Task<int> AddNewItemAsync(Item item);

        List<Item> GetItems();

        Task<Item> UpdateInfoOfItemAsync(int id, string status, int? price, string priceCurrency);

        Task<bool> IsTracked(string url);
    }
}
