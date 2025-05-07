using System.ComponentModel.DataAnnotations;

namespace stTrackerMVC.ViewModels
{
    public class AssignStudentsViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;

        //[Display(Name = "Студенты")]
        //public List<string> SelectedStudentIds { get; set; } = new List<string>();

        public List<StudentCheckboxItem> AvailableStudents { get; set; } = new List<StudentCheckboxItem>();
    }

    public class StudentCheckboxItem
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
