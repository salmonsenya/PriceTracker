using PriceTracker.Consts;
using System;

namespace PriceTracker.Helpers
{
    public class ShopDefiner : IShopDefiner
    {
        private const string NOT_DEFINED = "Unknown shop. Shop could not be defined from url.";
        public string GetShopName(string url)
        {
            if (url.Contains(PullAndBear.URL))
                return PullAndBear.SHOP_NAME;
            if (url.Contains(Bershka.URL))
                return Bershka.SHOP_NAME;
            else
            {
                throw new Exception(NOT_DEFINED);
            }
        }
    }
}
