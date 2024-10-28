using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            var entities = _context.Movies.Include(x => x.Genre).Include(x => x.ShowTimes)
                .AsQueryable();


            if(genre.HasValue)
                entities = entities.Where(x => x.GenreId == genre.Value);

            ViewBag.Genres = await _context.Genres.ToListAsync();

            return View(entities);
        }


        [HttpGet]

        public async Task<IActionResult> Create(Movie entity, bool isFirstLoad = true)
        {
            var genres = await _management.GetAll<Genre>();

            ViewBag.Genres = new SelectList(genres,"Id","Name");

            if(!ModelState.IsValid && isFirstLoad)
                ModelState.Clear();
            

            return View("CreateUpdate",entity);
        }


        [HttpPost]
        
        public async Task<IActionResult> CreatePost(Movie entity, IFormFile? imgFile)
        {

            if (ModelState.IsValid)
            {

                var isExisting = await _management.GetById<Movie>(entity.Id);

                if (isExisting == null)
                {

                    if (imgFile != null)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(imgFile.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("ImgFile", "Invalid image format. Please upload a JPG, JPEG, PNG, or GIF file.");
                            return RedirectToAction("Create", entity);
                        }

                        var relativePath = await _management.SaveImageToFileSystem(imgFile);
                        entity.ImgPath = relativePath;
                    }

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
                return RedirectToAction("Create", new {entity = entity, isFirstLoad = false });
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

        public async Task<IActionResult> UpdatePost(Movie entity, IFormFile? imgFile)
        {

            if (imgFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(imgFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImgFile", "Invalid image format. Please upload a JPG, JPEG, PNG, or GIF file.");
                }
                else
                {
                    var relativePath = await _management.SaveImageToFileSystem(imgFile);
                    entity.ImgPath = relativePath;
                }
            }


            if (ModelState.IsValid)
            {

                await _management.Update(entity);
                return RedirectToAction("Index");
            }
            else
            {
                var genres = await _management.GetAll<Genre>();
                ViewBag.Genres = new SelectList(genres, "Id", "Name");

                return View("CreateUpdate",entity);
            }

        }


        [HttpPost]

        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _management.GetById<Movie>(id);

            if(entity != null)
            {
                await _management.Delete<Movie>(entity.Id);
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }


        [HttpGet]

        public async Task<IActionResult> CreateShowTimes()
        {
            var movies = await _management.GetAll<Movie>();

            ViewBag.Movies = new SelectList(movies, "Id","Title");

            return View(new ShowTime() { Date = 
                new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.Today.Day+1),
                Time = new TimeSpan(15,0,0) //default value for date and time
                });
        }


        [HttpPost]

        public async Task<IActionResult> CreateShowTimes(ShowTime entity)
        {
            var allEntities = await _management.GetAll<ShowTime>();

            DateTime combined = entity.Date.Date + entity.Time;
            TimeSpan span = new TimeSpan(2, 30, 0);

            entity.ShowDate = combined;

            if(!allEntities.Any(x => x.ShowDate.Value.Date == entity.ShowDate.Value.Date
            && x.ShowDate.Value.TimeOfDay < entity.ShowDate.Value.TimeOfDay.Add(span)
            && x.ShowDate.Value.TimeOfDay > entity.ShowDate.Value.TimeOfDay.Subtract(span)))
            {
                await _management.Add(entity);
            }
            else
            {
                ModelState.AddModelError("Time", "Hours conflict! You need to have at least 2:30h time span between the show times");
                var movies = await _management.GetAll<Movie>();

                ViewBag.Movies = new SelectList(movies, "Id", "Title");
                return View();
            }

            return RedirectToAction("Index");
        }


        [HttpPost]

        public async Task<IActionResult> DeleteShowTime(int id)
        {
            var entity = await _management.GetById<ShowTime>(id);

            if(entity != null)
            {
                await _management.Delete<ShowTime>(entity.Id);
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

    }
}
