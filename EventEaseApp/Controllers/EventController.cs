using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
// this code was sourced from Juliana Adeola Adisa lessons and modified to fit the project

namespace EventEaseApp.Controllers
{
    public class EventController : Controller
    {
        private readonly EventEaseDBContext _context;

        public EventController(EventEaseDBContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var events = await (from e in _context.Event
                                join v in _context.Venue on e.VenueID equals v.VenueID
                                select new Event
                                {
                                    EventID = e.EventID,
                                    EventName = e.EventName,
                                    EventDate = e.EventDate,
                                    Description = e.Description,
                                    VenueName = v.VenueName
                                }).ToListAsync();

            return View(events);
        }



        public IActionResult Create()
        {
            PopulateVenueDropdown();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create(Event events)
        {
            if (!ModelState.IsValid)
            {
                // Log model state errors
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Field: {entry.Key}, Error: {error.ErrorMessage}");
                    }
                }

                PopulateVenueDropdown(); // Repopulate dropdown if validation fails
                return View(events);
            }

            _context.Add(events);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event created successfully!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? eventid)
        {
            if (eventid == null)
                return NotFound();

            var eventItem = _context.Event.Find(eventid);
            if (eventItem == null)
                return NotFound();

            // Populate venue list for the dropdown
            PopulateVenueDropdown();

            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int eventid, Event events)
        {
            if (eventid != events.EventID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(events);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(events.EventID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateVenueDropdown();  // Repopulate dropdown if validation fails
            return View(events);
        }

        private void PopulateVenueDropdown()
        {
            var venues = _context.Venue
                .OrderBy(v => v.VenueName)
                .Select(v => new { v.VenueID, v.VenueName })
                .ToList();

            ViewBag.VenueList = new SelectList(venues, "VenueID", "VenueName");
        }



        public async Task<IActionResult> Details(int? eventid)
        {
            if (eventid == null)
                return NotFound();

            var eventItem = await _context.Event.FindAsync(eventid);
            if (eventItem == null)
                return NotFound();

            // Fetch Venue Name using VenueID
            var venueName = await _context.Venue
                                           .Where(v => v.VenueID == eventItem.VenueID)
                                           .Select(v => v.VenueName)
                                           .FirstOrDefaultAsync();

            ViewBag.VenueName = venueName;  // Pass Venue Name to the view

            return View(eventItem);
        }


        public async Task<IActionResult> Delete(int eventid)
        {
            var eventItem = await _context.Event.FindAsync(eventid);
            if (eventItem == null)
                return NotFound();

            // Fetch Venue Name using VenueID
            var venueName = await _context.Venue
                                           .Where(v => v.VenueID == eventItem.VenueID)
                                           .Select(v => v.VenueName)
                                           .FirstOrDefaultAsync();

            ViewBag.VenueName = venueName;  // Pass Venue Name to the view

            return View(eventItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int eventid)
        {
            var eventItem = await _context.Event.FindAsync(eventid);
            if (eventItem == null)
                return NotFound();

            // Check if the event has any bookings associated with it
            var bookingsExist = await _context.Booking
                                               .AnyAsync(b => b.EventID == eventid);

            if (bookingsExist)
            {
                TempData["ErrorMessage"] = "Event cannot be deleted because it is linked to a booking.";
                return RedirectToAction(nameof(Index)); // Or return to the event details page
            }

            _context.Event.Remove(eventItem);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int eventid)
        {
            return _context.Event.Any(e => e.EventID == eventid);
        }

        


    }
}
