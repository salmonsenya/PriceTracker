namespace PriceTracker.Models
{
    public class ItemOnline
    {
        public int ItemId { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Status { get; set; }

        public int? Price { get; set; }

        public string PriceCurrency { get; set; }
    }
}
