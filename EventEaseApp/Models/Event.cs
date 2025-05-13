using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseApp.Models
{
    public class Event
    {
        public int EventID { get; set; }

        public int VenueID { get; set; }
        public string EventName { get; set; }
        public DateOnly EventDate { get; set; }
        public string Description { get; set; }
        
        [NotMapped]
        public string? VenueName { get; set; }

    }


}
