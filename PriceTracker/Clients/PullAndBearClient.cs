using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PriceTracker.Helpers;
using PriceTracker.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriceTracker.Clients
{
    public class PullAndBearClient : IPullAndBearClient
    {
        private HttpClient _httpClient;
        private readonly IParserHelper _parserHelper;

        public PullAndBearClient(HttpClient httpClient, IOptions<PullAndBearApiOptions> options, IParserHelper parserHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

            _parserHelper = parserHelper ?? throw new ArgumentNullException(nameof(parserHelper));
        }

        public async Task<TrackingStatus> GetItemInfoAsync(string url)
        {
            using (var httpResponseMessage = await _httpClient.GetAsync(new Uri(url)))
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw new Exception("The HTTP status code of the response was not expected (" + httpResponseMessage.StatusCode + ").");
                }
                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                var itemInfo = _parserHelper.GetItemInfo(responseString);

                return itemInfo;
            }
        }
    }
}
