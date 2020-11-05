using HtmlAgilityPack;
using Newtonsoft.Json;
using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public class ParserHelper : IParserHelper
    {
        public TrackingStatus GetItemInfo(string input)
        {
            TrackingStatus newInfo = null;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(input);
            var xpath = @"(//script[contains(@type,'application/ld+json')])[1]";
            var xpath2 = @"(//meta[contains(@property,'og:image')])[1]";
            var text = htmlDocument.DocumentNode.SelectSingleNode(xpath)?.InnerText;
            //var image = htmlDocument.DocumentNode.SelectSingleNode(xpath2).Attributes["content"].Value;
            if (text != null)
            {
                var product = JsonConvert.DeserializeObject<PullAndBearProduct>(text);
                newInfo = new TrackingStatus()
                {
                    Name = product.name,
                    Image = product.image, //!string.IsNullOrEmpty(product.image) ? product.image : image,
                    Status = null,
                    Price = int.Parse(product.offers.price),
                    PriceCurrency = product.offers.priceCurrency
                };
            }
            return newInfo;
        }
    }
}
