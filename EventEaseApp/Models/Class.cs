using System;
using System.Collections.Generic;

namespace EventEaseApp.Models
{
    public class EventSearchViewModel
    {
        // Filters
        public int? SelectedEventTypeId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? Availability { get; set; }

        // Dropdown options
        public List<EventType> EventTypes { get; set; }

        // Filtered results
        public List<Event> Events { get; set; }
    }
}

