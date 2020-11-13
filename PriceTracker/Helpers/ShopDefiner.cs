using PriceTracker.Consts;
using System;

namespace PriceTracker.Helpers
{
    public class ShopDefiner : IShopDefiner
    {
        public string GetShopName(string url)
        {
            if (url.Contains(PullAndBear.URL))
                return PullAndBear.SHOP_NAME;
            if (url.Contains(Bershka.URL))
                return Bershka.SHOP_NAME;
            throw new Exception(Exceptions.NOT_DEFINED_EXCEPTION);
        }
    }
}
