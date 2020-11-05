using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public interface IParserHelper
    {
        ItemOnline GetItemInfo(string input);
    }
}
