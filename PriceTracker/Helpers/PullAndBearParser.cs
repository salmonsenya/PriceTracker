using AutoMapper;
using PriceTracker.Consts;
using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public class PullAndBearParser : BershkaPullAndBearParser<PullAndBearProduct>, IPullAndBearParser
    {
        public PullAndBearParser(IMapper mapper) : base(mapper, PullAndBear.XPATH) { }
    }
}
