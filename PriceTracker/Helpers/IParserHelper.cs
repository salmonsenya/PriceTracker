using PriceTracker.Models;
using System.Threading.Tasks;

namespace PriceTracker.Helpers
{
    public interface IParserHelper
    {
        Task<TrackingStatus> GetItemInfoAsync(int itemId, string input);
    }
}
