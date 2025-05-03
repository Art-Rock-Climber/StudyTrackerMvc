using stTrackerMVC.Models;
using stTrackerMVC.Repositories;

namespace stTrackerMVC.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;

        public CourseService(ICourseRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await _repository.GetCoursesAsync();
        }

        public async Task AddCourseAsync(Course course)
        {
            await _repository.AddCourseAsync(course);
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _repository.GetCourseByIdAsync(id);
        }

        public async Task UpdateCourseAsync(Course course)
        {
            await _repository.UpdateCourseAsync(course);
        }

        public async Task DeleteCourseAsync(int id)
        {
            await _repository.DeleteCourseAsync(id);
        }

        public async Task<IEnumerable<Course>> GetCoursesWithTaskCountAsync(string? searchTerm = null)
        {
            return await _repository.GetCoursesAsync(searchTerm);
        }

        public async Task<Course> GetCourseWithTasksAsync(int id)
        {
            return await _repository.GetCourseByIdAsync(id, includeTasks: true);
        }
    }
}
