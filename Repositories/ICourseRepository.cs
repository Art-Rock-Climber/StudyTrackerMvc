using stTrackerMVC.Models;

namespace stTrackerMVC.Repositories
{
    public interface ICourseRepository
    {
        Task InitializeDataAsync();
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task AddCourseAsync(Course course);
        Task<Course?> GetCourseByIdAsync(int id);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);
    }
}
