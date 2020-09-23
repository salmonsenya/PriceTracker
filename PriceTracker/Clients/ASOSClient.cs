using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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


    }
}
