using PriceTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public class TrackingRepository : ITrackingRepository
    {
        public async Task AddNewItemAsync(Item item)
        {
            using (var db = new TrackingContext())
            {
                db.Items.Add(item);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<Item>> GetItems()
        {
            using (var db = new TrackingContext())
            {
                return db.Items.ToList();
            }
        }

        public async Task UpdateInfoOfItemAsync(int id, string status, int? price)
        {
            using (var db = new TrackingContext())
            {
                var item = db.Items.Where(i => i.ItemId == id).FirstOrDefault();
                item.Status = status;
                item.Price = price;
                await db.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTracked(string url)
        {
            using (var db = new TrackingContext())
            {
                var result = db.Items.Where(i => i.Url.Equals(url)).ToList();
                return result.Count > 0;
            }
        }
    }
}
