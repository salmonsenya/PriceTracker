using AutoMapper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PriceTracker.Models;
using System;

namespace PriceTracker.Helpers
{
    public abstract class BershkaPullAndBearParser<T>: IShopParser
    {
        private readonly IMapper _mapper;
        private const string PARSING_EXCEPTION = "Page could not be parsed.";
        private const string XPATH_EXCEPTION = "XML path could not be found on page.";
        private string _xpath;

        public BershkaPullAndBearParser(IMapper mapper, string xpath)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _xpath = xpath ?? throw new ArgumentNullException(nameof(xpath));
        }

        public ItemOnline GetItemInfo(string input)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(input);
            ItemOnline newInfo;
            try
            {
                var text = htmlDocument.DocumentNode.SelectSingleNode(_xpath)?.InnerText;
                if (text == null) throw new Exception(XPATH_EXCEPTION);
                var product = JsonConvert.DeserializeObject<T>(text);
                newInfo = _mapper.Map<T, ItemOnline>(product);
                newInfo.Status = null;
            }
            catch (Exception ex)
            {
                throw ex.Message.Equals(XPATH_EXCEPTION) ? new Exception(XPATH_EXCEPTION) : new Exception(PARSING_EXCEPTION);
            }
            return newInfo;
        }
    }
}
