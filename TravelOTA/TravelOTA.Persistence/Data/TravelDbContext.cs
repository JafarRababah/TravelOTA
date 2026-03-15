using Microsoft.EntityFrameworkCore;
using TravelOTA.Entitis.Domain;

namespace TravelOTA.Persistence.Data
{
    public class TravelDbContext : DbContext
    {

        public TravelDbContext(DbContextOptions<TravelDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<FlightSearch> FlightSearches { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Traveler> Travelers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // تعيين precision للـ decimal لتجنب تحذيرات EF Core
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);
        }
    }

}
