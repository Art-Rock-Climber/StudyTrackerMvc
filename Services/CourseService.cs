using stTrackerMVC.Models;
using stTrackerMVC.Repositories;

namespace stTrackerMVC.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;
        private readonly ITaskService _taskService;

        public CourseService(ICourseRepository repository, ITaskService taskService)
        {
            _repository = repository;
            _taskService = taskService;
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
            // Получаем все задания курса
            var course = await _repository.GetCourseByIdAsync(courseId, includeTasks: true);
            if (course == null) throw new Exception("Course not found");

            // Удаляем старые назначения
            await _repository.RemoveAllStudentsFromCourseAsync(courseId);

            // Добавляем новых студентов и создаем UserTasks
            foreach (var studentId in studentIds)
            {
                // Назначаем студента на курс
                await _repository.AddStudentToCourseAsync(courseId, studentId);

                // Создаем UserTasks для всех заданий курса
                foreach (var task in course.Tasks)
                {
                    await _taskService.CreateUserTaskIfNotExistsAsync(studentId, task.Id);
                }
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

        public async Task<IEnumerable<string>> GetAssignedStudentIdsAsync(int courseId)
        {
            return await _repository.GetCourseStudentIdsAsync(courseId);
        }
    }
}
