using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public interface IShopParser
    {
        ItemOnline GetItemInfo(string input);
    }
}
