namespace PriceTracker.Models
{
    public class ItemOnline
    {
        public int ItemId { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Status { get; set; }

        public float? Price { get; set; }

        public string PriceCurrency { get; set; }
    }
}
