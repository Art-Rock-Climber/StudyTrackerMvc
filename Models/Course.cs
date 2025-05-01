namespace stTrackerMVC.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ProfessorName { get; set; } = string.Empty;

        public Course() { }
    }
}
