using stTrackerMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace stTrackerMVC.ViewModels
{
    public class CourseTasksVm
    {
        public List<CourseTaskVm> Tasks { get; set; } = new();
        public int? CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string[] AvailableStatuses { get; set; } = Array.Empty<string>();
        public string? CurrentStatusFilter { get; set; }
        public string? CurrentSortOrder { get; set; }
    }

    public class CourseTaskVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int CourseId { get; set; }

        [Display(Name = "Мой статус")]
        public CourseTaskStatus Status { get; set; }
    }
}
