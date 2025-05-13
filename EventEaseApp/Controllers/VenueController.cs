using Azure.Storage.Blobs;
using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;


// this code was sourced from Juliana Adeola Adisa lessons and modified to fit the project
namespace EventEaseApp.Controllers
{
    public class VenueController : Controller
    {

        private readonly EventEaseDBContext _context;

        public VenueController(EventEaseDBContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            var venues = await _context.Venue.ToListAsync();
            return View(venues);
        }

        public IActionResult Create()
        {
            return View();
        }

        




        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload to Azure Blob Storage if an image file was provided
                // This is Step 4C: Modify Controller to receive ImageFile from View (user upload)
                // This is Step 5: Upload selected image to Azure Blob Storage
                if (venue.ImageFile != null)
                {

                    // Upload image to Blob Storage (Azure)
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile); //Main part of Step 5 B (upload image to Azure Blob Storage)

                    // Step 6: Save the Blob URL into ImageUrl property (the database)
                    venue.ImageUrl = blobUrl;
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }



        public async Task<IActionResult> Edit(int? venueid)
        {
            if (venueid == null)
                return NotFound();

            var venue = await _context.Venue.FindAsync(venueid);
            if (venue == null)
                return NotFound();

            return View(venue);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Venue venue)
        {
            if (!ModelState.IsValid)
                return View(venue);

            try
            {
                var existingVenue = await _context.Venue.AsNoTracking()
                                        .FirstOrDefaultAsync(v => v.VenueID == venue.VenueID);

                if (existingVenue == null)
                    return NotFound();

                if (venue.ImageFile != null)
                {
                    // New image uploaded: replace
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);
                    venue.ImageUrl = blobUrl;
                }
                else
                {
                    // No new image: keep existing one
                    venue.ImageUrl = existingVenue.ImageUrl;
                }

                _context.Update(venue);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Venue updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VenueExists(venue.VenueID))
                    return NotFound();
                else
                    throw;
            }
        }





        public async Task<IActionResult> Details(int? venueid)
        {
            if (venueid == null)
                return NotFound();

            var venue = await _context.Venue.FindAsync(venueid);
            if (venue == null)
                return NotFound();

            return View(venue);
        }




        public async Task<IActionResult> Delete(int venueid)
        {
            var venue = await _context.Venue.FindAsync(venueid);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int venueid)
        {
            var venue = await _context.Venue.FindAsync(venueid);

            if (venue == null)
                return NotFound();

            // Check if any bookings exist for this venue
            bool hasBookings = await _context.Booking.AnyAsync(b => b.VenueID == venueid);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue. It is associated with existing bookings.";
                return RedirectToAction(nameof(Index)); // Go back to venue list
            }

            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int venueid)
        {
            return _context.Venue.Any(e => e.VenueID == venueid);
        }


        // This is Step 5 (C): Upload selected image to Azure Blob Storage.
        // It completes the entire uploading process inside Step 5 â€” from connecting to Azure to returning the Blob URL after upload.
        // This will upload the Image to Blob Storage Account
        // Uploads an image to Azure Blob Storage and returns the Blob URL
        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=eventeasestorage10436124;AccountKey=/yKS4jvLRpQbW4fLlVenPBtU+rMDorQla9Pe2pgKvQzdvW4peamFOHeiZy0VFc5ZA3UZiBYMnhzQ+AStYKxAAQ==;EndpointSuffix=core.windows.net";
            var containerName = "eventeasecontainer";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }




    }
}
