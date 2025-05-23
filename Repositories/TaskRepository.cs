using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Data;
using stTrackerMVC.Models;
using System.Threading.Tasks;

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

            //if (status.HasValue)
            //{
            //    query = query.Where(t => t.Status == status.Value);
            //}

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
                //task.Status = status;
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

        public async Task UpdateUserTaskStatusAsync(int taskId, string? userId, CourseTaskStatus status)
        {
            var userTask = await _context.UserTasks
                .FirstOrDefaultAsync(ut => ut.TaskId == taskId && ut.UserId == userId);

            if (userTask == null)
            {
                userTask = new UserTask
                {
                    TaskId = taskId,
                    UserId = userId,
                    Status = status,
                    CompletedDate = status == CourseTaskStatus.Completed ? DateTime.Now : null
                };
                _context.UserTasks.Add(userTask);
            }
            else
            {
                userTask.Status = status;
                //if (status == CourseTaskStatus.Completed)
                //    userTask.CompletedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<UserTask?> GetUserTaskAsync(int taskId, string? userId)
        {
            return await _context.UserTasks
                .FirstOrDefaultAsync(ut => ut.TaskId == taskId && ut.UserId == userId);
        }

        public async Task<List<UserTask>> GetUserTasksForTasksAsync(string userId, List<int> taskIds)
        {
            return await _context.UserTasks
               .Where(ut => ut.UserId == userId && taskIds.Contains(ut.TaskId))
               .ToListAsync();
        }

        public async Task<List<UserTask>> GetOverdueTasksWithUsersAsync(DateTime currentDate)
        {
            var adminRoleId = await _context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            return await _context.UserTasks
                .Include(ut => ut.User)
                .Include(ut => ut.Task)
                    .ThenInclude(t => t.Course)
                .Where(ut => ut.Task.Deadline < currentDate &&
                             !_context.UserRoles.Any(ur => ur.UserId == ut.UserId && ur.RoleId == adminRoleId))
                .OrderBy(ut => ut.Task.Deadline)
                .ToListAsync();
        }

        public async Task<bool> UserTaskExistsAsync(string studentId, int taskId)
        {
            return await _context.UserTasks
                .AnyAsync(ut => ut.UserId == studentId && ut.TaskId == taskId);
        }

        public async Task AddUserTaskAsync(UserTask userTask)
        {
            await _context.UserTasks.AddAsync(userTask);
            await _context.SaveChangesAsync();
        }
    }
}
