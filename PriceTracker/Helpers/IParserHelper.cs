using PriceTracker.Models;
using System.Threading.Tasks;

namespace PriceTracker.Helpers
{
    public interface IParserHelper
    {
        Task<TrackingStatus> GetItemInfoAsync(string input);
    }
}
