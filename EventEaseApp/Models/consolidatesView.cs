namespace EventEaseApp.Models
{
    public class consolidatesView
    {

        public int BookingID { get; set; }
        public DateOnly BookingDate { get; set; }

        // Venue Info
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }

        // Event Info
        public string EventName { get; set; }
        public string EventDescription { get; set; }
    }
}
