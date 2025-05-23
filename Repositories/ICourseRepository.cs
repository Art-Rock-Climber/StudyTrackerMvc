using stTrackerMVC.Models;

namespace stTrackerMVC.Repositories
{
    public interface ICourseRepository
    {
        // Основные методы работы с курсами
        Task InitializeDataAsync();
        Task<IEnumerable<Course>> GetCoursesAsync(string? searchTerm = null);
        Task AddCourseAsync(Course course);
        Task<Course?> GetCourseByIdAsync(int id, bool includeTasks = false);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);

        // Методы для работы с назначением студентов
        Task<bool> IsStudentAssignedAsync(string studentId, int courseId);
        Task RemoveAllStudentsFromCourseAsync(int courseId);
        Task AddStudentToCourseAsync(int courseId, string studentId);
        Task SaveChangesAsync();
        Task<List<string>> GetCourseStudentIdsAsync(int courseId);
        IQueryable<Course> GetCoursesForStudentQueryable(string studentId);

        Task<IEnumerable<AppUser>> GetStudentsNotInListAsync(IEnumerable<string> excludedIds);
        //Task<List<CourseStudent>> GetAllStudentsAsync();
        //Task<List<string>> GetAssignedStudentIdsAsync(int courseId);
        //Task UpdateCourseStudentsAsync(int courseId, List<string> assignedStudentIds);
    }
}
