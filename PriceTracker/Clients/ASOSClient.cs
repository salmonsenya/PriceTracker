using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PriceTracker.Helpers;
using PriceTracker.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracker.Clients
{
    public class ASOSClient : IASOSClient
    {
        private HttpClient _httpClient;
        private readonly IParserHelper _parserHelper;

        public ASOSClient(HttpClient httpClient, IOptions<ASOSApiOptions> options, IParserHelper parserHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _parserHelper = parserHelper ?? throw new ArgumentNullException(nameof(parserHelper));
        }

        public async Task<TrackingStatus> GetItemInfoAsync(string paramUrl)
        {
            var url = $@"";
            var data = JsonConvert.SerializeObject(paramUrl);
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            using (var httpResponseMessage = await _httpClient.PostAsync(new Uri(url), content))
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw new Exception("The HTTP status code of the response was not expected (" + httpResponseMessage.StatusCode + ").");
                }
                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                var itemInfo = await _parserHelper.GetItemInfoAsync(responseString);

                return null;
            }
        }
    }
}
