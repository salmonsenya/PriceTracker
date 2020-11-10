using PriceTracker.Models;
using System.Threading.Tasks;

namespace PriceTracker.Clients
{
    public interface IShopClient
    {
        Task<ItemOnline> GetItemInfoAsync(string paramUrl);
    }
}
