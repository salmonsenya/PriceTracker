using AutoMapper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PriceTracker.Models;
using System;

namespace PriceTracker.Helpers
{
    public class ParserHelper : IParserHelper
    {
        private readonly IMapper _mapper;

        public ParserHelper(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ItemOnline GetItemInfo(string input)
        {
            ItemOnline newInfo = null;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(input);
            var xpath = @"(//script[contains(@type,'application/ld+json')])[1]";
            var xpath2 = @"(//meta[contains(@property,'og:image')])[1]";
            var text = htmlDocument.DocumentNode.SelectSingleNode(xpath)?.InnerText;
            //var image = htmlDocument.DocumentNode.SelectSingleNode(xpath2).Attributes["content"].Value;
            if (text != null)
            {
                var product = JsonConvert.DeserializeObject<PullAndBearProduct>(text);
                newInfo = _mapper.Map<PullAndBearProduct, ItemOnline>(product);
                newInfo.Status = null;
            }
            return newInfo;
        }
    }
}
