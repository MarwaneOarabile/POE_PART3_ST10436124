using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseApp.Models
{
    public class Venue
    {

        public int VenueID { get; set; }

        [Required]
        public string VenueName { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public int Capacity { get; set; }

        // stores url of the image
        public string ImageUrl { get; set; }

        // stores the image
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
