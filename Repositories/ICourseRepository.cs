using stTrackerMVC.Models;

namespace stTrackerMVC.Repositories
{
    public interface ICourseRepository
    {
        Task InitializeDataAsync();
        Task<IEnumerable<Course>> GetCoursesAsync(string? searchTerm = null);
        Task AddCourseAsync(Course course);
        Task<Course?> GetCourseByIdAsync(int id, bool includeTasks = false);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);
    }
}
