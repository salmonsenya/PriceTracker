using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace PriceTracker.Models
{
    public class TrackingContext : DbContext
    {

        public DbSet<Item> Items { get; set; }

        // public TrackingContext(DbContextOptions<TrackingContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql("Server=localhost;User Id=tracker-admin;Password=tracker-password;Database=TrackingItemsDb"); //_configuration.GetConnectionString("TrackingContext")
    }
}
