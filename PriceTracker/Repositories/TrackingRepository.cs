using PriceTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace PriceTracker.Repositories
{
    public class TrackingRepository : ITrackingRepository
    {
        public int AddNewItem(Item item)
        {
            using (var _trackingContext = new TrackingContext())
            {
                _trackingContext.Items.Add(item);
                _trackingContext.SaveChanges();
                var result = _trackingContext.Items.Where(i => i.Url == item.Url).FirstOrDefault();
                return result.ItemId;
            }
        }

        public List<Item> GetItems() {
            using (var _trackingContext = new TrackingContext())
            {
                var items = _trackingContext.Items;
                return items.ToList();
            }
        }
          
        public void UpdateInfoOfItem(int id, string status, int? price, string priceCurrency, string name, string image)
        {
            using (var _trackingContext = new TrackingContext())
            {
                var item = _trackingContext.Items.Where(i => i.ItemId == id).FirstOrDefault();
                item.Status = status;
                item.Price = price;
                item.PriceCurrency = priceCurrency;
                item.Name = name;
                item.Image = image;
                _trackingContext.SaveChanges();
                var changed = _trackingContext.Items.ToList().Find(x => x.ItemId == id);
                var x = changed;
            }
        }

        public bool IsTracked(string url)
        {
            using (var _trackingContext = new TrackingContext())
            {
                return _trackingContext.Items.Where(i => i.Url.Equals(url)).ToList().Count > 0;
            }
        }
    }
}
