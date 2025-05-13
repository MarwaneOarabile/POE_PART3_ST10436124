using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EventEaseApp.Models;
using Microsoft.EntityFrameworkCore;


namespace EventEaseApp.Controllers;
// this code was sourced from Juliana Adeola Adisa lessons and modified to fit the project

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly EventEaseDBContext _context;

    public HomeController(ILogger<HomeController> logger, EventEaseDBContext context)
    {
        _logger = logger;
        _context = context;
    }

    

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> BookingOverview()
    {
        var overview = await _context.BookingOverviewView.ToListAsync();
        return View(overview);
    }

    public async Task<IActionResult> Search(string searchTerm)
    {
        ViewData["SearchTerm"] = searchTerm; // Preserve the term in the search box

        var query = from b in _context.Booking
                    join e in _context.Event on b.EventID equals e.EventID
                    join v in _context.Venue on b.VenueID equals v.VenueID
                    select new consolidatesView
                    {
                        BookingID = b.BookingID,
                        BookingDate = b.BookingDate,
                        EventName = e.EventName,
                        EventDescription = e.Description,
                        VenueName = v.VenueName,
                        VenueLocation = v.Location
                    };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string loweredTerm = searchTerm.ToLower();
            query = query.Where(b =>
                b.BookingID.ToString().Contains(searchTerm) ||
                b.EventName.ToLower().Contains(loweredTerm));
        }

        var results = await query.ToListAsync();

        return View(results);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
