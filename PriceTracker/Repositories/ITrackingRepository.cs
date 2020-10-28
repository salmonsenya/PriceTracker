using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public interface ITrackingRepository
    {
        int AddNewItem(Item item);

        List<Item> GetItems();

        void UpdateInfoOfItem(int id, string status, int? price, string priceCurrency, string name, string image);

        bool IsTracked(string url);
    }
}
