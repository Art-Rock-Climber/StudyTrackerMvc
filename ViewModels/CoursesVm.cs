using stTrackerMVC.Models;

namespace stTrackerMVC.ViewModels
{
    public class CoursesVm
    {
        public List<Course> Courses { get; set; }

        public CoursesVm(List<Course> cources)
        {
            this.Courses = cources;
        }
    }
}

