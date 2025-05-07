using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Models;
using stTrackerMVC.Services;
using stTrackerMVC.ViewModels;

namespace stTrackerMVC.ViewModelBuilders
{
    public class CoursesVmBuilder
    {
        private readonly ICourseService _courseService;
        private readonly UserManager<AppUser> _userManager;

        public CoursesVmBuilder(ICourseService courseService, UserManager<AppUser> userManager)
        {
            _courseService = courseService;
            _userManager = userManager;
        }

        //public async Task<CoursesVm> GetCoursesVmAsync()
        //{
        //    var courses = await _courseService.GetCoursesAsync();
        //    return new CoursesVm(courses.ToList());
        //}

        public async Task<CoursesVm> Build(string? searchTerm = null)
        {
            var courses = await _courseService.GetCoursesWithTaskCountAsync(searchTerm);

            return new CoursesVm
            {
                Courses = courses.Select(c => new CourseVm
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProfessorName = c.ProfessorName,
                }).ToList(),
                SearchTerm = searchTerm
            };
        }

        public async Task<CourseVm> BuildOne(int id)
        {
            var course = await _courseService.GetCourseWithTasksAsync(id);

            return new CourseVm
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                ProfessorName = course.ProfessorName,
                Tasks = course.Tasks.Select(t => new CourseTaskVm
                {
                    Id = t.Id,
                    Title = t.Title,
                    Deadline = t.Deadline
                }).ToList()
            };
        }

        public async Task<CoursesVm> BuildForStudent(string studentId, string searchTerm = null)
        {
            var query = _courseService.GetCoursesForStudent(studentId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(searchTerm) ||
                    c.Description.Contains(searchTerm) ||
                    c.ProfessorName.Contains(searchTerm));
            }

            var courses = await query.ToListAsync();

            return new CoursesVm
            {
                Courses = courses.Select(c => new CourseVm
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProfessorName = c.ProfessorName
                }).ToList(),
                SearchTerm = searchTerm
            };
        }

        public async Task<AssignStudentsViewModel> BuildAssignStudentsViewModel(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return null;

            var availableStudents = await _courseService.GetStudentsNotInCourseAsync(courseId);

            return new AssignStudentsViewModel
            {
                CourseId = course.Id,
                CourseName = course.Name,
                AvailableStudents = availableStudents.Select(s => new StudentCheckboxItem
                {
                    Id = s.Id,
                    Name = $"{s.FirstName} {s.LastName}",
                    IsSelected = false
                }).ToList()
            };
        }
    }
}

