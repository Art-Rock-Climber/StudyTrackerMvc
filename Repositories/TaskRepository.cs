using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Data;
using stTrackerMVC.Models;

namespace stTrackerMVC.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CourseTask> GetByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<List<CourseTask>> GetByCourseIdAsync(int courseId, CourseTaskStatus? status = null)
        {
            var query = _context.Tasks
            .Include(t => t.Course)
            .Where(t => t.CourseId == courseId);

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            return await query
                .OrderBy(t => t.Deadline)
                .ToListAsync();
        }

        public async Task CreateAsync(CourseTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseTask task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await GetByIdAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(int taskId, CourseTaskStatus status)
        {
            var task = await GetByIdAsync(taskId);
            if (task != null)
            {
                task.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CourseTask>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.Course)
                .OrderBy(t => t.Deadline)
                .ToListAsync();
        }
    }
}
