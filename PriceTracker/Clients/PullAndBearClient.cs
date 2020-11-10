using Microsoft.Extensions.Options;
using PriceTracker.Helpers;
using PriceTracker.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriceTracker.Clients
{
    public class PullAndBearClient : IShopClient
    {
        private HttpClient _httpClient;
        private readonly IParser _parserHelper;
        private const string HTTP_EXCEPTION = "The HTTP status code of the response was not expected";

        public PullAndBearClient(HttpClient httpClient, IOptions<PullAndBearApiOptions> options, IParser parserHelper)
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
                {
                    throw new Exception($"{HTTP_EXCEPTION} ({httpResponseMessage.StatusCode}).");
                }
                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                var itemInfo = _parserHelper.GetItemInfo(responseString);
                return itemInfo;
            }
        }
    }
}
