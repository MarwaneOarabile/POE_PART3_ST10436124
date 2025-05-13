namespace EventEaseApp.Models
{
    public class BookingOverview
    {
        public int BookingID { get; set; }
        public DateOnly BookingDate { get; set; }

        public int EventID { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }

        public int VenueID { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }
    }
}
