using Microsoft.EntityFrameworkCore;
using PriceTracker.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public class TrackingRepository : ITrackingRepository
    {
        public async Task<int> AddNewItemAsync(Item item)
        {
            using var _trackingContext = new TrackingContext();
            _trackingContext.Items.Add(item);
            await _trackingContext.SaveChangesAsync();
            var result = await _trackingContext.Items.Where(i => i.Url == item.Url).FirstOrDefaultAsync();
            return result.ItemId;
        }

        public async Task<List<Item>> GetItemsAsync() {
            using var _trackingContext = new TrackingContext();
            return await _trackingContext.Items.ToListAsync();
        }
          
        public async Task UpdateInfoOfItemAsync(int id, string status, int? price, string priceCurrency, string name, string image)
        {
            using var _trackingContext = new TrackingContext();
            var item = await _trackingContext.Items.Where(i => i.ItemId == id).FirstOrDefaultAsync();
            item.Status = status;
            item.Price = price;
            item.PriceCurrency = priceCurrency;
            item.Name = name;
            item.Image = image;
            await _trackingContext.SaveChangesAsync();
        }

        public async Task<bool> IsTracked(string url, int userId)
        {
            using var _trackingContext = new TrackingContext();
            var items = await _trackingContext.Items.Where(i => i.Url.Equals(url) && i.UserId == userId).ToListAsync();
            return items.Count > 0;
        }
    }
}
