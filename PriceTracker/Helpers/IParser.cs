using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public interface IParser
    {
        ItemOnline GetItemInfo(string input);
    }
}
