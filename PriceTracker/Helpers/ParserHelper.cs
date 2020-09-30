using Newtonsoft.Json;
using PriceTracker.Models;
using System.Threading.Tasks;

namespace PriceTracker.Helpers
{
    public class ParserHelper : IParserHelper
    {
        public async Task<TrackingStatus> GetItemInfoAsync(string input)
        {
            return JsonConvert.DeserializeObject<TrackingStatus>(input);
        }
    }
}
