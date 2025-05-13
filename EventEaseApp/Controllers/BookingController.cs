using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

// this code was sourced from Juliana Adeola Adisa lessons and modified to fit the project

namespace EventEaseApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly EventEaseDBContext _context;

        public BookingController(EventEaseDBContext context)
        {
            _context = context;
        }

        
        public async Task<ActionResult> Index()
        {
            var bookings = await _context.Booking.ToListAsync();
            return View(bookings);
        }


        public IActionResult Create()
        {
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName");
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            // Check if the selected Event exists
            var selectedEvent = await _context.Event.FirstOrDefaultAsync(e => e.EventID == booking.EventID);
            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
                ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                // Check for existing booking with same venue + event date
                bool isDoubleBooked = await _context.Booking.AnyAsync(b =>
                    b.VenueID == booking.VenueID &&
                    b.BookingDate == selectedEvent.EventDate);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "This venue is already booked on the event date.");
                    ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
                    ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
                    return View(booking);
                }

                try
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                }
            }

            // Reload dropdowns before returning view (in all error cases)
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }



        public async Task<IActionResult> Edit(int? bookingid)
        {
            if (bookingid == null)
                return NotFound();

            var booking = await _context.Booking.FindAsync(bookingid);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int bookingid, Booking booking)
        {
            if (bookingid != booking.BookingID)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Check for existing booking with same venue + date (excluding current booking)
                bool isDoubleBooked = await _context.Booking.AnyAsync(b =>
                    b.BookingID != booking.BookingID &&
                    b.VenueID == booking.VenueID &&
                    b.BookingDate == booking.BookingDate);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "This venue is already booked on the selected date.");
                    // Reload dropdown lists before returning view
                    ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
                    ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
                    return View(booking);
                }

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdown lists before returning view (if model is invalid)
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }


        public async Task<IActionResult> Details(int? bookingid)
        {
            if (bookingid == null)
                return NotFound();

            var booking = await _context.Booking.FindAsync(bookingid);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        
        public async Task<IActionResult> Delete(int bookingid)
        {
            var booking = await _context.Booking.FindAsync(bookingid);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int bookingid)
        {
            var booking = await _context.Booking.FindAsync(bookingid);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int bookingid)
        {
            return _context.Booking.Any(e => e.BookingID == bookingid);
        }
    }
}
