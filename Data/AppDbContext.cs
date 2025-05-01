using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Models;

namespace stTrackerMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация модели
            modelBuilder.Entity<Course>().ToTable("Courses");

            // Пример начальных данных
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Name = "Math", Description = "some description", ProfessorName = "Ivanov" },
                new Course { Id = 2, Name = "History", Description = "world history", ProfessorName = "Petrov" },
                new Course { Id = 3, Name = "Computer Science", Description = null, ProfessorName = "Sidorov" }
            );
        }
    }
}
