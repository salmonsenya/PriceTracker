using Microsoft.EntityFrameworkCore;

namespace PriceTracker.Models
{
    public class TrackingContext : DbContext
    {
        public DbSet<Item> Items { get; set; }

        public TrackingContext(DbContextOptions<TrackingContext> options) : base(options) { }
    }
}
