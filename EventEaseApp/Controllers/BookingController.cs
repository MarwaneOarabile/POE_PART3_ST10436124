using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEaseApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly EventEaseDBContext _context;

        public BookingController(EventEaseDBContext context)
        {
            _context = context;
        }

        // GET: Booking Index
        public async Task<ActionResult> Index()
        {
            var bookings = await _context.Booking.ToListAsync();
            return View(bookings);
        }

        // GET: Booking Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Booking Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        // GET: Booking Edit
        public async Task<IActionResult> Edit(int? bookingid)
        {
            if (bookingid == null)
                return NotFound();

            var booking = await _context.Booking.FindAsync(bookingid);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Booking Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int bookingid, Booking booking)
        {
            if (bookingid != booking.BookingID)
                return NotFound();

            if (ModelState.IsValid)
            {
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
            return View(booking);
        }

        // GET: Booking Details
        public async Task<IActionResult> Details(int? bookingid)
        {
            if (bookingid == null)
                return NotFound();

            var booking = await _context.Booking.FindAsync(bookingid);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // GET: Booking Delete
        public async Task<IActionResult> Delete(int bookingid)
        {
            var booking = await _context.Booking.FindAsync(bookingid);
            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Booking Delete
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
