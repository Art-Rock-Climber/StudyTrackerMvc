namespace stTrackerMVC.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ProfessorName { get; set; }

        public Course(int id, string name, string? description, string profName)
        {
            Id = id;
            Name = name;
            Description = description;
            ProfessorName = profName;
        }
    }
}
