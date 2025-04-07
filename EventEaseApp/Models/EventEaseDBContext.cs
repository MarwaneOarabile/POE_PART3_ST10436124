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


    }
}
