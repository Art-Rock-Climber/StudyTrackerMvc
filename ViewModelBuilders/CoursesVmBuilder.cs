using stTrackerMVC.Models;
using stTrackerMVC.Services;
using stTrackerMVC.ViewModels;

namespace stTrackerMVC.ViewModelBuilders
{
    public class CoursesVmBuilder
    {
        private readonly ICourseService _courseService;

        public CoursesVmBuilder(ICourseService courseService)
        {
            _courseService = courseService;
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
    }
}

