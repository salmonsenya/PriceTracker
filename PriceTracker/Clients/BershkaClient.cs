using Microsoft.Extensions.Options;
using PriceTracker.Helpers;
using System.Net.Http;

namespace PriceTracker.Clients
{
    public class BershkaClient : ShopClient, IBershkaClient
    {
        public BershkaClient(HttpClient httpClient, IOptions<PullAndBearApiOptions> options, IBershkaParser parserHelper)
            : base(httpClient, options, parserHelper){ }
    }
}
