using stTrackerMVC.Models;

namespace stTrackerMVC.Repositories
{
    public class CourseRepository
    {
        private List<Course> _courses;
        public CourseRepository()
        {
            SetDefaultCourses();
        }

        public void SetDefaultCourses()
        {
            var course1 = new Course(id: 1, "Math", "some description", "Ivanov");
            var course2 = new Course(id: 2, "History", "world history", "Petrov");
            var course3 = new Course(id: 3, "Computer Science", description: null, "Sidorov");

            _courses = [course1, course2, course3];
        }

        public List<Course> GetCourses()
        {
            return _courses;
        }

        public void AddCourse(Course course)
        {
            _courses.Add(course);
        }
    }
}
