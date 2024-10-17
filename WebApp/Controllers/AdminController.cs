﻿using Data;
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
        public async Task<IActionResult> Index()
        {
            var entities = await _context.Movies.Include(x => x.Genre).ToListAsync();

            return View(entities);
        }


        [HttpGet]

        public async Task<IActionResult> Create(Movie entity)
        {
            var genres = await _management.GetAll<Genre>();

            ViewBag.Genres = new SelectList(genres,"Id","Name");


            return View(entity);
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

    }
}