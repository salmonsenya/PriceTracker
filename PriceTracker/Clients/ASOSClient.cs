using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PriceTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PriceTracker.Clients
{
    public class ASOSClient : IASOSClient
    {
        private HttpClient _httpClient;

        public ASOSClient(HttpClient httpClient, IOptions<ASOSApiOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<TrackingStatus> GetItemInfoAsync(string paramUrl)
        {
            var url = $@"";
            var data = JsonConvert.SerializeObject(paramUrl);
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            using (var httpResponseMessage = await _httpClient.PostAsync(new Uri(url), content))
            {
                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw new Exception("The HTTP status code of the response was not expected (" + httpResponseMessage.StatusCode + ").");
                }

                // var info = JsonConvert.DeserializeObject<TrackingStatus>(responseString);
                return null;
            }
        }
    }
}
