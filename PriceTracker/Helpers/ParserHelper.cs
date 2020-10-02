using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PriceTracker.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PriceTracker.Helpers
{
    public class ParserHelper : IParserHelper
    {
        public async Task<TrackingStatus> GetItemInfoAsync(int itemId, string input)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(input);
            var xpath = @"(//script[contains(@type,'application/ld+json')])[1]";
            var text = htmlDocument.DocumentNode.SelectSingleNode(xpath).InnerText;
            var product = JsonConvert.DeserializeObject<PullAndBearProduct>(text);
            var newInfo = new TrackingStatus()
            {
                ItemId = itemId,
                Status = null,
                Price = int.Parse(product.offers.price),
                PriceCurrency = product.offers.priceCurrency
            };
            return newInfo;
        }
    }
}
