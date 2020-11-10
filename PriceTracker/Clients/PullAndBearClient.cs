using Microsoft.Extensions.Options;
using PriceTracker.Helpers;
using System.Net.Http;

namespace PriceTracker.Clients
{
    public class PullAndBearClient : ShopClient, IPullAndBearClient
    {
        public PullAndBearClient(HttpClient httpClient, IOptions<PullAndBearApiOptions> options, IPullAndBearParser parserHelper)
            : base(httpClient, options, parserHelper) { }
    }
}
