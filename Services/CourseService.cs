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


        public async Task<bool> IsStudentAssignedToCourseAsync(string studentId, int courseId)
        {
            return await _repository.IsStudentAssignedAsync(studentId, courseId);
        }

        public async Task AssignStudentsToCourseAsync(int courseId, IEnumerable<string> studentIds)
        {
            // Удаляем старые назначения
            await _repository.RemoveAllStudentsFromCourseAsync(courseId);

            // Добавляем новых студентов
            foreach (var studentId in studentIds)
            {
                await _repository.AddStudentToCourseAsync(courseId, studentId);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppUser>> GetStudentsNotInCourseAsync(int courseId)
        {
            var studentsInCourse = await _repository.GetCourseStudentIdsAsync(courseId);
            return await _repository.GetStudentsNotInListAsync(studentsInCourse);
        }

        public IQueryable<Course> GetCoursesForStudent(string studentId)
        {
            return _repository.GetCoursesForStudentQueryable(studentId);
        }
    }
}
