﻿namespace PriceTracker.Models
{
    public class TrackingStatus
    {
        public int ItemId { get; set; }

        public string Status { get; set; }

        public int? Price { get; set; }
    }
}