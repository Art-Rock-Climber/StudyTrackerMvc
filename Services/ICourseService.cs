using stTrackerMVC.Models;

namespace stTrackerMVC.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task AddCourseAsync(Course course);
        Task<Course?> GetCourseByIdAsync(int id);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);

        Task<IEnumerable<Course>> GetCoursesWithTaskCountAsync(string? searchTerm = null);
        Task<Course> GetCourseWithTasksAsync(int id);

        Task<bool> IsStudentAssignedToCourseAsync(string studentId, int courseId);
        Task AssignStudentsToCourseAsync(int courseId, IEnumerable<string> studentIds);
        Task<IEnumerable<AppUser>> GetStudentsNotInCourseAsync(int courseId);

        IQueryable<Course> GetCoursesForStudent(string studentId);
    }
}
