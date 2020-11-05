﻿using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public class UpdateInfoHelper : IUpdateInfoHelper
    {
        public Item GetUpdatedItem(Item item, ItemOnline newInfo)
        {
            var updatedItem = item;
            updatedItem.Status = newInfo.Status;
            updatedItem.Price = newInfo.Price;
            updatedItem.PriceCurrency = newInfo.PriceCurrency;
            updatedItem.Name = newInfo.Name;
            updatedItem.Image = newInfo.Image;
            return updatedItem;
        }
    }
}