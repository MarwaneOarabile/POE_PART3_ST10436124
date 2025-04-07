namespace EventEaseApp.Models
{
    public class Event
    {
        
        public int EventID { get; set; }

        public int VenueID { get; set; }

        public string EventName { get; set; }

        public DateOnly EventDate { get; set; }

        public string Description { get; set; }
    }
}
