using PriceTracker.Models;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public interface ITrackingRepository
    {
        Task AddNewItemAsync(Item item);

        Task UpdateInfoOfItemAsync(int id, string status, int? price);

        Task<bool> IsTracked(string url);
    }
}
