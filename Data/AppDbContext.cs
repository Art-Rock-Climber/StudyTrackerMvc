using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Models;

namespace stTrackerMVC.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseTask> Tasks { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация для Course
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");

                // Настройка автоинкремента
                entity.Property(c => c.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.ProfessorName)
                      .HasMaxLength(100);

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
                entity.ToTable("Tasks");

                entity.Property(t => t.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Deadline)
                      .IsRequired();

                entity.Property(t => t.Status)
                      .HasConversion<string>()
                      .IsRequired();

                // Связь с курсом
                entity.HasOne(t => t.Course)
                      .WithMany(c => c.Tasks)
                      .HasForeignKey(t => t.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация для связи Course-Student
            modelBuilder.Entity<CourseStudent>(entity =>
            {
                entity.ToTable("CourseStudents");

                entity.HasKey(cs => new { cs.CourseId, cs.StudentId });

                // Связь с курсом
                entity.HasOne(cs => cs.Course)
                      .WithMany(c => c.CourseStudents)
                      .HasForeignKey(cs => cs.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Связь с пользователем
                entity.HasOne(cs => cs.Student)
                      .WithMany(u => u.CourseStudents)
                      .HasForeignKey(cs => cs.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            //// Переименование таблиц Identity (опционально)
            //modelBuilder.Entity<AppUser>(entity =>
            //{
            //    entity.ToTable("Users");
            //});

            //modelBuilder.Entity<IdentityRole>(entity =>
            //{
            //    entity.ToTable("Roles");
            //});

            //modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            //{
            //    entity.ToTable("UserRoles");
            //});

            //modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            //{
            //    entity.ToTable("UserClaims");
            //});

            //modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            //{
            //    entity.ToTable("UserLogins");
            //});

            //modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            //{
            //    entity.ToTable("RoleClaims");
            //});

            //modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            //{
            //    entity.ToTable("UserTokens");
            //});
        }
    }
}
