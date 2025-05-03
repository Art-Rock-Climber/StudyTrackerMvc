using stTrackerMVC.Models;

namespace stTrackerMVC.ViewModels
{
    public class CoursesVm
    {
        public List<CourseItemVm> Courses { get; set; } = new();
        public int TotalCount => Courses.Count;
        public string? SearchTerm { get; set; } // Для фильтрации

        //public CoursesVm(List<CourseItemVm> courses, string searchTerm)
        //{
        //    Courses = courses;
        //    SearchTerm = searchTerm;
        //}
    }

    public class CourseItemVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProfessorName { get; set; }
        public int TaskCount { get; set; }
    }
}

