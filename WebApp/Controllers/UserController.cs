using Data;
using Data.Models;
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
        public async Task<IActionResult> Index(int? genre, DateTime? dateFrom, DateTime? dateTo)
        {

            var films = _context.ShowTimes.Include(x => x.Movie).
                ThenInclude(x => x.Genre).Where(x => x.ShowDate.HasValue
                && x.ShowDate >= DateTime.Today);

            if (genre.HasValue)
                films = films.Where(x => x.Movie.GenreId == genre);

            if(dateFrom.HasValue)
                films = films.Where(x => x.ShowDate >= dateFrom);

            if (dateTo.HasValue)
                films = films.Where(x => x.ShowDate <= dateTo);

            ViewBag.Genres = await _management.GetAll<Genre>();


            return View(films);
        }
    }
}
