using Microsoft.Extensions.Options;
using PriceTracker.Consts;
using PriceTracker.Helpers;
using PriceTracker.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriceTracker.Clients
{
    public abstract class ShopClient : IShopClient
    {
        private HttpClient _httpClient;
        private readonly IShopParser _parserHelper;
        

        public ShopClient(HttpClient httpClient, IOptions<PullAndBearApiOptions> options, IShopParser parserHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

            _parserHelper = parserHelper ?? throw new ArgumentNullException(nameof(parserHelper));
        }

        public async Task<ItemOnline> GetItemInfoAsync(string url)
        {
            using (var httpResponseMessage = await _httpClient.GetAsync(new Uri(url)))
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                    throw new Exception($"{Exceptions.HTTP_EXCEPTION} ({httpResponseMessage.StatusCode}).");
                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                var itemInfo = _parserHelper.GetItemInfo(responseString);
                return itemInfo;
            }
        }
    }
}
