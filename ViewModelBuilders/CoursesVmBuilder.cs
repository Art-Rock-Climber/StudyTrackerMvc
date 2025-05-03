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

        public async Task<CoursesVm> Build(string searchTerm = null)
        {
            var courses = await _courseService.GetCoursesWithTaskCountAsync(searchTerm);

            return new CoursesVm
            {
                Courses = courses.Select(c => new CourseItemVm
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProfessorName = c.ProfessorName,
                    TaskCount = c.Tasks.Count
                }).ToList(),
                SearchTerm = searchTerm
            };
        }
    }
}

