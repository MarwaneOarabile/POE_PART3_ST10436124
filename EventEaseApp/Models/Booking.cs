namespace EventEaseApp.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        public int EventID { get; set; }
        public int VenueID { get; set; }
        public DateOnly BookingDate { get; set; }


    }
}
