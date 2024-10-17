using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly IMovieManagement _management;
        private readonly MovieReservationSystemContext _context;
        public AdminController(IMovieManagement management, MovieReservationSystemContext _context)
        {
            _management = management;
            this._context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entities = await _management.GetAll<Movie>();

            return View(entities);
        }


        [HttpGet]

        public IActionResult Create()
        {
            return View(new Movie());
        }


        [HttpPost]
        
        public async Task<IActionResult> Create(Movie entity)
        {

            if (ModelState.IsValid)
            {

                var isExisting = await _management.GetById<Movie>(entity.Id);

                if (isExisting == null)
                {
                    await _management.Add(entity);
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return BadRequest();
            }
        }

    }
}
