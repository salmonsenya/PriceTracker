using AutoMapper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PriceTracker.Consts;
using PriceTracker.Models;
using System;

namespace PriceTracker.Helpers
{
    public class BershkaParser : IBershkaParser
    {
        private readonly IMapper _mapper;
        private const string PARSING_EXCEPTION = "Page could not be parsed.";
        private const string XPATH_EXCEPTION = "XML path could not be found on page.";
        private readonly string xpath = Bershka.XPATH;

        public BershkaParser(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ItemOnline GetItemInfo(string input)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(input);
            ItemOnline newInfo;
            try
            {
                var text = htmlDocument.DocumentNode.SelectSingleNode(xpath)?.InnerText;
                if (text == null) throw new Exception(XPATH_EXCEPTION);
                var product = JsonConvert.DeserializeObject<BershkaProduct>(text);
                newInfo = _mapper.Map<BershkaProduct, ItemOnline>(product);
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
