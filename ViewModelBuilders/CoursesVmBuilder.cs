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

        public async Task<CoursesVm> GetCoursesVmAsync()
        {
            var courses = await _courseService.GetCoursesAsync();
            return new CoursesVm(courses.ToList());
        }
    }
}

