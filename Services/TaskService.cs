using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Models;
using stTrackerMVC.Repositories;

namespace stTrackerMVC.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public Task<CourseTask> GetTaskAsync(int id) => _repository.GetByIdAsync(id);

        public async Task<List<CourseTask>> GetTasksByCourseAsync(int courseId, string? statusFilter = null)
        {
            CourseTaskStatus? status = null;
            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<CourseTaskStatus>(statusFilter, out var parsedStatus))
            {
                status = parsedStatus;
            }

            return await _repository.GetByCourseIdAsync(courseId, status);
        }

        public async Task CreateTaskAsync(CourseTask task)
        {
            if (task.Deadline < DateTime.Today)
                throw new ArgumentException("Дедлайн не может быть в прошлом");

            await _repository.CreateAsync(task);
        }

        public async Task UpdateTaskAsync(CourseTask task)
        {
            if (task.Deadline < DateTime.Today)
                throw new ArgumentException("Дедлайн не может быть в прошлом");

            await _repository.UpdateAsync(task);
        }

        public Task DeleteTaskAsync(int id) => _repository.DeleteAsync(id);

        public Task UpdateTaskStatusAsync(int taskId, CourseTaskStatus status) =>
            _repository.UpdateStatusAsync(taskId, status);

        public async Task<List<CourseTask>> GetUpcomingDeadlinesAsync(int days = 7)
        {
            var allTasks = await _repository.GetAllAsync();
            return allTasks
                .Where(t => t.Deadline <= DateTime.Now.AddDays(days) &&
                            t.Deadline >= DateTime.Now &&
                            t.Status != CourseTaskStatus.Completed)
                .OrderBy(t => t.Deadline)
                .ToList();
        }

        public async Task<IEnumerable<CourseTask>> GetAllTasksAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
