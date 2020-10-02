using PriceTracker.Models;
using System.Threading.Tasks;

namespace PriceTracker.Helpers
{
    public interface IParserHelper
    {
        TrackingStatus GetItemInfo(int itemId, string input);
    }
}
