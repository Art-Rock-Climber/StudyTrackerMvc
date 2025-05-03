namespace stTrackerMVC.ViewModels
{
    public class TasksVm
    {
        public List<TaskItemVm> Tasks { get; set; } = new();
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }

    public class TaskItemVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Deadline { get; set; }
    }
}
