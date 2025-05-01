using stTrackerMVC.Models;
using stTrackerMVC.Repositories;

namespace stTrackerMVC.Services
{
    public class CourseService
    {
        private readonly CourseRepository _repository;

        public CourseService()
        {
            _repository = new CourseRepository();
        }

        public List<Course> GetCourses()
        {
            return _repository.GetCourses();
        }

        public void AddCourse(Course course)
        {
            _repository.AddCourse(course);
        }
    }
}
