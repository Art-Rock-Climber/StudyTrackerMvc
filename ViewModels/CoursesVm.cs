using System.ComponentModel.DataAnnotations;

namespace stTrackerMVC.ViewModels
{
    public class CoursesVm
    {
        public List<CourseVm> Courses { get; set; } = new();
        public int TotalCount => Courses.Count;
        public string? SearchTerm { get; set; } // Для фильтрации
    }

    public class CourseVm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Укажите преподавателя")]
        [StringLength(100)]
        public string ProfessorName { get; set; } = string.Empty;

        public List<CourseTaskVm> Tasks { get; set; } = new();
    }
}

