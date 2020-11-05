using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public interface IParserHelper
    {
        TrackingStatus GetItemInfo(string input);
    }
}
