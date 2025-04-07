using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: Edit
        public async Task<IActionResult> Edit(int? venueid)
        {
            if (venueid == null)
                return NotFound();

            var venue = await _context.Venue.FindAsync(venueid);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Edit
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
            // Find the venue by ID
            var venue = await _context.Venue.FindAsync(venueid);

            // Check if the venue exists
            if (venue == null)
            {
                // If not, return a NotFound result
                return NotFound();
            }

            // If the venue exists, remove it from the context and save changes
            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();

            // Redirect to the index page after deletion
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int venueid)
        {
            return _context.Venue.Any(e => e.VenueID == venueid);
        }





    }
}
