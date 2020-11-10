using AutoMapper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PriceTracker.Models;
using System;
using PriceTracker.Consts;

namespace PriceTracker.Helpers
{
    public class PullAndBearParser : IParser
    {
        private readonly IMapper _mapper;
        private const string PARSING_EXCEPTION = "Page could not be parsed.";
        private const string XPATH_EXCEPTION = "XML path could not be found on page.";
        private readonly string xpath = PullAndBear.XPATH;

        public PullAndBearParser(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ItemOnline GetItemInfo(string input)
        {
            ItemOnline newInfo = null;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(input);
            try
            {
                var text = htmlDocument.DocumentNode.SelectSingleNode(xpath)?.InnerText;
                if (text == null) throw new Exception(XPATH_EXCEPTION);
                if (text != null)
                {
                    var product = JsonConvert.DeserializeObject<PullAndBearProduct>(text);
                    newInfo = _mapper.Map<PullAndBearProduct, ItemOnline>(product);
                    newInfo.Status = null;
                }
            } catch (Exception ex)
            {
                throw ex.Message.Equals(XPATH_EXCEPTION) ? new Exception(XPATH_EXCEPTION) : new Exception(PARSING_EXCEPTION);
            }
            return newInfo;
        }
    }
}
