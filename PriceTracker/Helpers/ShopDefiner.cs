using PriceTracker.Consts;
using System;

namespace PriceTracker.Helpers
{
    public class ShopDefiner : IShopDefiner
    {
        private const string NOT_DEFINED = "Item could not be added for tracking. Shop could not be defined from url.";
        public string GetShopName(string url)
        {
            if (url.Contains(PullAndBear.URL))
                return PullAndBear.SHOP_NAME;
            if (url.Contains(Bershka.URL))
                return Bershka.SHOP_NAME;
            throw new Exception(NOT_DEFINED);
        }
    }
}
