using Microsoft.EntityFrameworkCore;

namespace EventEaseApp.Models
{
    public class EventEaseDBContext : DbContext
    {
        public EventEaseDBContext(DbContextOptions<EventEaseDBContext> options) : base(options)
        {

        }

        public DbSet<Venue> Venue {get; set;}
        public DbSet<Event> Event {get; set;}
        public DbSet<Booking> Booking { get; set; }

        public DbSet<EventType> EventType {get; set;}

        public DbSet<BookingOverview> BookingOverviewView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookingOverview>()
                .HasNoKey()
                .ToView("BookingOverviewView");
        }


    }
}
