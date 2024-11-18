using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieReservationSystemContext _context;
        public HomeController(ILogger<HomeController> logger, MovieReservationSystemContext _context)
        {
            _logger = logger;
            this._context = _context;
        }

        public IActionResult Index()
        {
            //displays show times in upcoming two weeks

            var films = _context.ShowTimes.Include(x => x.Movie).
                ThenInclude(x => x.Genre).Where(x => x.ShowDate.HasValue
                && x.ShowDate >= DateTime.Today && x.ShowDate <= DateTime.Today.AddDays(30));

            return View(films);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
