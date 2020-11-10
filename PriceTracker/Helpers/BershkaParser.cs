using AutoMapper;
using PriceTracker.Consts;
using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public class BershkaParser : BershkaPullAndBearParser<BershkaProduct>, IBershkaParser
    {
        public BershkaParser(IMapper mapper) : base(mapper, Bershka.XPATH) { }
    }
}
