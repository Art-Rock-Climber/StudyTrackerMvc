using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Data;
using stTrackerMVC.Models;

namespace stTrackerMVC.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CourseRepository(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task InitializeDataAsync()
        {
            if (!await _context.Courses.AnyAsync())
            {
                var initialCourses = new List<Course>
                {
                    new Course
                    {
                        Id = 1,
                        Name = "Математика",
                        Description = "Основы алгебры и геометрии",
                        ProfessorName = "Иванов И.И."
                    },
                    new Course
                    {
                        Id = 2,
                        Name = "История",
                        Description = "Всемирная история",
                        ProfessorName = "Петрова П.П."
                    },
                    new Course
                    {
                        Id = 3,
                        Name = "Информатика",
                        Description = "Основы программирования",
                        ProfessorName = "Сидоров С.С."
                    }
                };

                await _context.Courses.AddRangeAsync(initialCourses);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(string? searchTerm = null)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm) ||
                                       c.ProfessorName.Contains(searchTerm));
            }

            return await query.ToListAsync();
        }

        public async Task AddCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id, bool includeTasks = false)
        {
            var query = _context.Courses.AsQueryable();

            if (includeTasks)
                query = query.Include(c => c.Tasks);

            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await GetCourseByIdAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> IsStudentAssignedAsync(string studentId, int courseId)
        {
            return await _context.CourseStudents
                .AnyAsync(cs => cs.StudentId == studentId && cs.CourseId == courseId);
        }

        public async Task RemoveAllStudentsFromCourseAsync(int courseId)
        {
            var assignments = await _context.CourseStudents
                .Where(cs => cs.CourseId == courseId)
                .ToListAsync();

            _context.CourseStudents.RemoveRange(assignments);
        }

        public async Task AddStudentToCourseAsync(int courseId, string studentId)
        {
            var assignment = new CourseStudent
            {
                CourseId = courseId,
                StudentId = studentId
            };

            await _context.CourseStudents.AddAsync(assignment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetCourseStudentIdsAsync(int courseId)
        {
            return await _context.CourseStudents
                .Where(cs => cs.CourseId == courseId)
                .Select(cs => cs.StudentId)
                .ToListAsync();
        }

        public IQueryable<Course> GetCoursesForStudentQueryable(string studentId)
        {
            return _context.CourseStudents
                .Where(cs => cs.StudentId == studentId)
                .Include(cs => cs.Course)
                .ThenInclude(c => c.Tasks) // Если нужно включать задачи
                .Select(cs => cs.Course);
        }


        public async Task<IEnumerable<AppUser>> GetStudentsNotInListAsync(IEnumerable<string> excludedIds)
        {
            // Получаем всех пользователей с ролью "Student", которых нет в excludedIds
            var studentRoleName = "Student"; // Убедитесь, что такая роль существует
            var studentUsers = await _userManager.GetUsersInRoleAsync(studentRoleName);

            return studentUsers
                .Where(u => !excludedIds.Contains(u.Id))
                .ToList();
        }
    }
}
