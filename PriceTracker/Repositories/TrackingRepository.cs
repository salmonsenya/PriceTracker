using PriceTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public class TrackingRepository : ITrackingRepository
    {

        private readonly TrackingContext _trackingContext;

        public TrackingRepository(TrackingContext trackingContext)
        {
            _trackingContext = trackingContext ?? throw new ArgumentNullException(nameof(trackingContext));
        }

        public async Task<int> AddNewItemAsync(Item item)
        {
            _trackingContext.Items.Add(item);
            await _trackingContext.SaveChangesAsync();
            var result = _trackingContext.Items.Where(i => i.Url == item.Url).FirstOrDefault();
            return result.ItemId;
        }

        public List<Item> GetItems() => 
            _trackingContext.Items.ToList();

        public async Task<Item> UpdateInfoOfItemAsync(int id, string status, int? price, string priceCurrency)
        {
            var item = _trackingContext.Items.Where(i => i.ItemId == id).FirstOrDefault();
            item.Status = status;
            item.Price = price;
            item.PriceCurrency = priceCurrency;
            await _trackingContext.SaveChangesAsync();
            var newItem = _trackingContext.Items.Where(i => i.ItemId == id).FirstOrDefault();
            return newItem;
        }

        public async Task<bool> IsTracked(string url) => 
            _trackingContext.Items.Where(i => i.Url.Equals(url)).ToList().Count > 0;
    }
}
