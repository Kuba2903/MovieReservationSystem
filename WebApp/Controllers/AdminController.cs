using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index(int? genre)
        {
            var entities = _context.Movies.Include(x => x.Genre).AsQueryable();


            if(genre.HasValue)
                entities = entities.Where(x => x.GenreId == genre.Value);

            ViewBag.Genres = await _context.Genres.ToListAsync();

            return View(entities);
        }


        [HttpGet]

        public async Task<IActionResult> Create(Movie entity)
        {
            var genres = await _management.GetAll<Genre>();

            ViewBag.Genres = new SelectList(genres,"Id","Name");


            return View("CreateUpdate",entity);
        }


        [HttpPost]
        
        public async Task<IActionResult> CreatePost(Movie entity)
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
                entity.IsValid = false;
                return RedirectToAction("Create",entity);
            }
        }


        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            var entity = await _management.GetById<Movie>(id);

            if(entity != null)
            {
                var genres = await _management.GetAll<Genre>();

                ViewBag.Genres = new SelectList(genres, "Id", "Name");

                return View("CreateUpdate",entity);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]

        public async Task<IActionResult> UpdatePost(Movie entity)
        {
            if (ModelState.IsValid)
            {
                await _management.Update(entity);
                return RedirectToAction("Index");
            }
            else
            {
                var genres = await _management.GetAll<Genre>();
                ViewBag.Genres = new SelectList(genres, "Id", "Name");

                entity.IsValid = false;
                return View("CreateUpdate",entity);
            }

        }


        [HttpPost]

        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _management.GetById<Movie>(id);

            if(entity != null)
            {
                _context.Movies.Remove(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

    }
}
