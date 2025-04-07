using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
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

       
        public async Task<ActionResult> Index()
        {
            var events = await _context.Event.ToListAsync();
            return View(events);
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event events)
        {
            if (ModelState.IsValid)
            {
                _context.Add(events);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(events);
        }

        public async Task<IActionResult> Edit(int? eventid)
        {
            if (eventid == null)
                return NotFound();

            var eventItem = await _context.Event.FindAsync(eventid);
            if (eventItem == null)
                return NotFound();

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

            return View(events);
        }

        
        public async Task<IActionResult> Details(int? eventid)
        {
            if (eventid == null)
                return NotFound();

            var eventItem = await _context.Event.FindAsync(eventid);
            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }

       
        public async Task<IActionResult> Delete(int eventid)
        {
            var eventItem = await _context.Event.FindAsync(eventid);
            if (eventItem == null)
                return NotFound();

            return View(eventItem);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int eventid)
        {
            var eventItem = await _context.Event.FindAsync(eventid);
            if (eventItem != null)
            {
                _context.Event.Remove(eventItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int eventid)
        {
            return _context.Event.Any(e => e.EventID == eventid);
        }
    }
}
