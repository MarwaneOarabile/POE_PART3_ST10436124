using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
                _context.Add(venue);
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(int venueid, Venue venue)
        {
            if (venueid != venue.VenueID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
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





    }
}
