using Microsoft.EntityFrameworkCore;

namespace PriceTracker.Models
{
    public class TrackingContext : DbContext
    {

        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql("Server=localhost;User Id=tracker-admin;Password=tracker-password;Database=TrackingItemsDb"); //_configuration.GetConnectionString("TrackingContext")
    }
}
