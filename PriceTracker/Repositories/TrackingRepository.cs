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

        public async Task AddNewItemAsync(Item item)
        {
            _trackingContext.Items.Add(item);
            await _trackingContext.SaveChangesAsync();
        }

        public async Task<List<Item>> GetItems() => 
            _trackingContext.Items.ToList();

        public async Task UpdateInfoOfItemAsync(int id, string status, int? price)
        {
            var item = _trackingContext.Items.Where(i => i.ItemId == id).FirstOrDefault();
            item.Status = status;
            item.Price = price;
            await _trackingContext.SaveChangesAsync();
        }

        public async Task<bool> IsTracked(string url) => 
            _trackingContext.Items.Where(i => i.Url.Equals(url)).ToList().Count > 0;
    }
}
