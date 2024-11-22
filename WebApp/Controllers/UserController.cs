using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    [Authorize(Roles = "User")]
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
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? genre, DateTime? dateFrom, DateTime? dateTo)
        {

            var films = await _context.ShowTimes.Include(x => x.Movie).
                ThenInclude(x => x.Genre).Where(x => x.ShowDate.HasValue
                && x.ShowDate >= DateTime.Today).GroupBy(x => x.Movie)
                .ToDictionaryAsync(g => g.Key,
                                        g => g.Select(x => x.ShowDate).ToList());

            if (genre.HasValue)
                films = films.Where(x => x.Key.GenreId == genre).ToDictionary(g => g.Key,
                    g => g.Value);

            if (dateFrom.HasValue)
            {
                films = films
                    .Select(kvp => new KeyValuePair<Movie, List<DateTime?>>(
                        kvp.Key,
                        kvp.Value.Where(d => d >= dateFrom.Value).ToList()
                    ))
                    .Where(kvp => kvp.Value.Any()) // Ensure only movies with matching dates are included
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }


            if (dateTo.HasValue)
            {
                films = films
                    .Select(kvp => new KeyValuePair<Movie, List<DateTime?>>(
                        kvp.Key,
                        kvp.Value.Where(d => d <= dateTo.Value).ToList()
                    ))
                    .Where(kvp => kvp.Value.Any()) // Ensure only movies with matching dates are included
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            ViewBag.Genres = await _management.GetAll<Genre>();
            ViewBag.UserId = await GetUser();

            return View(films);
        }


        [HttpGet]

        public async Task<IActionResult> AddReservation(int showTimeId, string? errorMessage)
        {
            var sectors = await _context.SeatReservations.Where(x => x.ShowTimeId == showTimeId)
                .Select(x => x.Sector).Distinct().ToListAsync();
            
            var seats = await _context.SeatReservations.Where(x => x.ShowTimeId == showTimeId)
                .Select(x => x.Seat).Distinct().ToListAsync();

            var occupiedSeats = await _context.SeatReservations
                .Where(x => x.ShowTimeId == showTimeId && x.UserId != null)
                .GroupBy(x => x.Sector)
                .ToDictionaryAsync(g => g.Key, g => g.Select(seat => seat.Seat).ToList());

            var film = await _context.SeatReservations.Include(x => x.ShowTime).ThenInclude(x => x.Movie)
                .Where(x => x.ShowTimeId == showTimeId).Select(x => x.ShowTime.Movie).FirstOrDefaultAsync();
            
            if(!string.IsNullOrEmpty(errorMessage))
                ViewBag.ErrorMessage = errorMessage;

            ViewBag.Film = film.Title;
            ViewBag.seatsLeft = (sectors.Count * seats.Count) - occupiedSeats.Count;
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

                /// Validates if the user previously booked the seat for this movie

                var isContainingUser = await _context.SeatReservations.FirstOrDefaultAsync(x => x.ShowTimeId == entity.ShowTimeId
                && x.UserId == id);

                if (isContainingUser == null)
                {
                    existingReservation.UserId = id;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    return RedirectToAction("AddReservation", new { showTimeId  = entity.ShowTimeId, 
                        errorMessage = "This user already booked his seat for this movie!"
                    });
                }
            }

            return RedirectToAction("Index");
        }


        [HttpGet]

        public async Task<IActionResult> CheckReservations()
        {
            string userId = await GetUser();

            ViewBag.UserId = userId;

            var user = await _context.Users.Include(x => x.SeatReservations).
                FirstOrDefaultAsync(x => x.Id == userId);

            var showTimesId = user.SeatReservations.Select(x => x.ShowTimeId).ToList();


            var films = await _context.ShowTimes.Include(x => x.Movie).ThenInclude(x => x.Genre)
                .Where(x => showTimesId.Contains(x.Id) && x.ShowDate >= DateTime.Now).ToListAsync();

            var reservations = await _context.SeatReservations.Where(x => showTimesId.Contains(x.ShowTimeId))
                .ToListAsync();

            ViewBag.reservations = reservations;

            return View(films);
        }


        [HttpGet]
        public async Task<IActionResult> CancelReservation(int showTimeId, string userId, string? errorMessage)
        {
            var findReservation = await _context.SeatReservations.Include(x => x.ShowTime).
                ThenInclude(x => x.Movie).
                FirstOrDefaultAsync(x => x.ShowTimeId == showTimeId && x.UserId == userId);

            ViewBag.film = findReservation.ShowTime.Movie.Title;
            ViewBag.showTimeDate = findReservation.ShowTime.ShowDate.Value.ToString("yyyy-MM-dd HH:mm");

            if(!string.IsNullOrEmpty(errorMessage))
                ViewBag.errorMessage = errorMessage;

            if (findReservation != null)
                return View(findReservation);
            else
                return NotFound();
        }


        [HttpPost]

        public async Task<IActionResult> CancelReservation(SeatReservation findReservation)
        {
            TimeSpan hour = new TimeSpan(36000000000);
            var hourFromNow = DateTime.Now.TimeOfDay.Add(hour);
            
            var showDate = await _context.ShowTimes.FirstOrDefaultAsync(x =>
                 x.SeatReservations.Contains(findReservation) && x.ShowDate.HasValue &&
                 x.ShowDate.Value.Hour > hourFromNow.TotalHours);

            if (showDate != null)
            {
                _context.SeatReservations.Remove(findReservation);
                await _context.SaveChangesAsync();
            }
            else
            {
                return RedirectToAction("CancelReservation", new
                {
                    showTimeId = findReservation.ShowTimeId,
                    userId = findReservation.UserId,
                    errorMessage = "You are able to cancel at least one hour" +
                    " before the start of the movie"
                });
            }

            return RedirectToAction("CheckReservations");
        }
    }
}
