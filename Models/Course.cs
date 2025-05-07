using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stTrackerMVC.Models
{
    public class Course
    {
        //[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Укажите преподавателя")]
        [StringLength(100)]
        public string ProfessorName { get; set; } = string.Empty;

        public ICollection<CourseTask> Tasks { get; set; } = new List<CourseTask>();
        public ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();
    }
}
