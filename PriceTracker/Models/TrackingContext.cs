using Microsoft.EntityFrameworkCore;

namespace PriceTracker.Models
{
    public class TrackingContext : DbContext
    {
        public TrackingContext(DbContextOptions<TrackingContext> options)
        : base(options)
        { }

        public DbSet<Item> Items { get; set; }

        public DbSet<UserStatus> UserStatuses { get; set; }
    }
}
