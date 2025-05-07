using Microsoft.AspNetCore.Identity;

namespace stTrackerMVC.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Связь с курсами (для студентов)
        public ICollection<CourseStudent> CourseStudents { get; set; }
    }

    public class CourseStudent
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string StudentId { get; set; }
        public AppUser Student { get; set; }
    }

}
