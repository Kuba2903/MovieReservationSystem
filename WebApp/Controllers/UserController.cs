using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly MovieReservationSystemContext _context;

        private readonly IMovieManagement _management;

        public UserController(MovieReservationSystemContext context, IMovieManagement management)
        {
            _context = context;
            _management = management;
        }

        [HttpGet]
        public IActionResult Index()
        {

            var films = _context.ShowTimes.Include(x => x.Movie).
                ThenInclude(x => x.Genre).Where(x => x.ShowDate.HasValue
                && x.ShowDate >= DateTime.Today);


            return View(films);
        }
    }
}
