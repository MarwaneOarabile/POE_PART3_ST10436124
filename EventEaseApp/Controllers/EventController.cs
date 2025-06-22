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


        public async Task<IActionResult> Index(string searchType, int? venueId, DateOnly? startDate, DateOnly? endDate, bool? available) // ✅ NEW: added availability
        {
            var events = _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchType))
            {
                events = events.Where(e => e.EventType.Name == searchType);
            }

            if (venueId.HasValue)
            {
                events = events.Where(e => e.VenueID == venueId.Value);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                events = events.Where(e => e.EventDate >= startDate && e.EventDate <= endDate);
            }

            if (available.HasValue) // ✅ NEW: availability filtering
            {
                events = events.Where(e => e.Venue.IsAvailable == available.Value);
            }

            // Provide dropdown filter options
            ViewData["EventType"] = _context.EventType.ToList();
            ViewData["Venues"] = _context.Venue.ToList();

            // ✅ Store selected values in ViewBag to pre-fill the form in the view
            ViewBag.SelectedType = searchType;
            ViewBag.SelectedVenue = venueId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Availability = available;

            return View(await events.ToListAsync());
        }






        public IActionResult Create()
        {
            PopulateVenueDropdown();
            ViewData["Venues"] = _context.Venue.ToList();
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
                ViewData["EvetTypes"] = _context.EventType.ToList();

                return View(events);
            }

            //
            

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

            ViewData["EvetTypes"] = _context.EventType.ToList();
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
