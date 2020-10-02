namespace PriceTracker.Models
{
    public class PullAndBearProduct
    {
        public string logo { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string mpn { get; set; }

        public string brand { get; set; }

        public PullAndBearOffer offers { get; set; }
    }
}
