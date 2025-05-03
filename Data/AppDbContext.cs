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
        public DbSet<CourseTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация для Course
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");

                // Настройка автоинкремента
                entity.Property(c => c.Id)
                      .ValueGeneratedOnAdd();

                // Начальные данные
                entity.HasData(
                    new Course { Id = 1, Name = "Math", Description = "some description", ProfessorName = "Ivanov" },
                    new Course { Id = 2, Name = "History", Description = "world history", ProfessorName = "Petrov" },
                    new Course { Id = 3, Name = "Computer Science", Description = null, ProfessorName = "Sidorov" }
                );
            });

            // Конфигурация для CourseTask
            modelBuilder.Entity<CourseTask>(entity =>
            {
                // Настройка автоинкремента
                entity.Property(t => t.Id)
                      .ValueGeneratedOnAdd();

                // Конвертация enum в строку
                entity.Property(t => t.Status)
                      .HasConversion<string>();
            });
        }
    }
}
