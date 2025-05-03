using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stTrackerMVC.Models
{
    public class CourseTask
    {
        //[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Укажите дедлайн")]
        public DateTime Deadline { get; set; }

        [Required]
        public CourseTaskStatus Status { get; set; } = CourseTaskStatus.NotStarted;

        //[ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course? Course { get; set; }
    }

    public enum CourseTaskStatus
    {
        [Display(Name = "Не начато")]
        NotStarted,

        [Display(Name = "В процессе")]
        InProgress,

        [Display(Name = "Завершено")]
        Completed
    }
}
