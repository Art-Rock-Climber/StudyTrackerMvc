using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Data;
using stTrackerMVC.Models;

namespace stTrackerMVC.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
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
    }
}
