using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    //[Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly MovieReservationSystemContext _context;

        private readonly IMovieManagement _management;

        private UserManager<ApplicationUser> _userManager;

        public UserController(MovieReservationSystemContext context, IMovieManagement management,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _management = management;
            _userManager = userManager;
        }

        public async Task<string> GetUser()
        {
            var user = await _userManager.GetUserAsync(User);

            if(user != null)
                return user.Id;
            else
                return string.Empty;
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
            ViewBag.UserId = await GetUser();

            return View(films);
        }


        [HttpGet]

        public async Task<IActionResult> AddReservation(int showTimeId)
        {
            var sectors = await _context.SeatReservations.Where(x => x.ShowTimeId == showTimeId)
                .Select(x => x.Sector).Distinct().ToListAsync();
            
            var seats = await _context.SeatReservations.Where(x => x.ShowTimeId == showTimeId)
                .Select(x => x.Seat).Distinct().ToListAsync();

            var occupiedSeats = await _context.SeatReservations
                .Where(x => x.ShowTimeId == showTimeId && x.UserId != null)
                .GroupBy(x => x.Sector)
                .ToDictionaryAsync(g => g.Key, g => g.Select(seat => seat.Seat).ToList());

            ViewBag.Sectors = sectors;
            ViewBag.Seats = seats;
            ViewBag.Occup = occupiedSeats;

            return View(new SeatReservation() { ShowTimeId = showTimeId});
        }

        [HttpPost]

        public async Task<IActionResult> AddReservation(SeatReservation entity, string sector, string seat)
        {
            var existingReservation = await _context.SeatReservations
                .FirstOrDefaultAsync(x => x.ShowTimeId == entity.ShowTimeId && x.Sector == sector && x.Seat == seat);

            if (existingReservation != null)
            {
                string? id = await GetUser();
                existingReservation.UserId = id;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
