using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EventEaseApp.Models
{
    public class Event
    {
        public int EventID { get; set; }

        public int VenueID { get; set; }

        public Venue Venue { get; set; }
        public string EventName { get; set; }
        public DateOnly EventDate { get; set; }
        public string Description { get; set; }
        
        [NotMapped]
        public string? VenueName { get; set; }


        public int? EventTypeID { get; set; }

        public EventType? EventType { get; set; }

    }


}
