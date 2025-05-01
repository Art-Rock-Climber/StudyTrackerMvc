using stTrackerMVC.Models;
using stTrackerMVC.Services;
using stTrackerMVC.ViewModels;

namespace stTrackerMVC.ViewModelBuilders
{
    public class CoursesVmBuilder
    {
        private readonly CourseService _courseService;
        public CoursesVmBuilder()
        {
            _courseService = new CourseService();
        }

        public CoursesVm GetCoursesVm()
        {
            var courses = _courseService.GetCourses();
            return new CoursesVm(courses);
        }
    }
}

