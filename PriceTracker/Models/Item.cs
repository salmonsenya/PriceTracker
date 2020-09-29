using System;

namespace PriceTracker.Models
{
    public class Item
    {
        public int ItemId { get; set; }

        public string Url { get; set; }

        public string Status { get; set; }

        public int? Price { get; set; }

        public DateTime SatrtTrackingDate { get; set; }

        public string Source { get; set; }

        public long ChatId { get; set; }
    }
}
