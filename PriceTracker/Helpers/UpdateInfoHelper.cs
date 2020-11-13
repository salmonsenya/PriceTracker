using PriceTracker.Models;
using System;

namespace PriceTracker.Helpers
{
    public class UpdateInfoHelper : IUpdateInfoHelper
    {
        public void GetUpdatedItem(ref Item item, ItemOnline newInfo)
        {
            item.Status = newInfo.Status;
            item.Price = newInfo.Price;
            item.PriceCurrency = newInfo.PriceCurrency;
            item.Name = newInfo.Name;
            item.Image = newInfo.Image;
            item.LastUpdateDate = DateTime.Now;
        }

        public void GetUpdatedItem(ref Item item, Item newInfo)
        {
            item.Status = newInfo.Status;
            item.Price = newInfo.Price;
            item.PriceCurrency = newInfo.PriceCurrency;
            item.Name = newInfo.Name;
            item.Image = newInfo.Image;
            item.LastUpdateDate = DateTime.Now;
        }
    }
}
