
namespace EventEaseApp.Models
{
    public class EventType
    {
        public int EventTypeID { get; set; }
        public string? Name { get; set; }

        public ICollection<Event> Events { get; set; }


    }
}
