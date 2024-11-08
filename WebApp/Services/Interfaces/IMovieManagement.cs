using Data.Models;

namespace WebApp.Services.Interfaces
{
    public interface IMovieManagement
    {
        Task Add<T>(T obj) where T : class;

        Task Update<T>(T obj) where T : class;

        Task Delete<T>(int id) where T : class;

        Task<T> GetById<T>(int id) where T : class;

        Task<List<T>> GetAll<T>() where T : class;

        Task<string> SaveImageToFileSystem(IFormFile image);

        Task FillSeats_Sectors(int showTimeId);
    }
}
