using Data;
using Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations
{
    public class MovieManagement : IMovieManagement
    {
        private readonly MovieReservationSystemContext _context;

        public MovieManagement(MovieReservationSystemContext context)
        {
            _context = context;
        }

        public async Task Add<T>(T obj) where T : class
        {
            await _context.Set<T>().AddAsync(obj);
            await _context.SaveChangesAsync();
        }

        public async Task Delete<T>(int id) where T : class
        {
            var x = await _context.Set<T>().FindAsync(id);

            if(x != null)
            {
                _context.Set<T>().Remove(x);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<T>> GetAll<T>() where T : class
        {
            var items = await _context.Set<T>().ToListAsync();
            return items;
        }

        public async Task<T> GetById<T>(int id) where T : class
        {
            var item = await _context.Set<T>().FindAsync(id);

            return item;
        }

        public async Task Update<T>(T obj) where T : class
        {
            _context.Set<T>().Update(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<string> SaveImageToFileSystem(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine("wwwroot/uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";
        }

        public async Task FillSeats_Sectors(int showTimeId)
        {
            string[] sectors = { "A","B","C","D","E","F","G","H" };

            string[] seats = { "0", "1", "2", "3", "4", "5", "6", "7","8","9" };


            for (int i = 0; i < sectors.Length; i++)
            {
                for (int j = 0; j < seats.Length; j++)
                {
                    SeatReservation entity = new SeatReservation()
                    {
                        ShowTimeId = showTimeId,
                        Sector = sectors[i],
                        Seat = seats[j]
                    };

                    await _context.AddAsync(entity);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
