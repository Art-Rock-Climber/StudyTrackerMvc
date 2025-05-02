using System.ComponentModel.DataAnnotations;

namespace stTrackerMVC.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Укажите преподавателя")]
        public string ProfessorName { get; set; } = string.Empty;

        public Course() { }
    }
}
