using stTrackerMVC.Models;

namespace stTrackerMVC.Services
{
    public interface ITaskService
    {
        Task<CourseTask> GetTaskAsync(int id);
        Task<List<CourseTask>> GetTasksByCourseAsync(int courseId, string? statusFilter = null);
        Task<IEnumerable<CourseTask>> GetAllTasksAsync();
        Task CreateTaskAsync(CourseTask task);
        Task UpdateTaskAsync(CourseTask task);
        Task DeleteTaskAsync(int id);
        Task UpdateTaskStatusAsync(int taskId, CourseTaskStatus status);
        Task<List<CourseTask>> GetUpcomingDeadlinesAsync(int days = 7);
        Task UpdateUserTaskStatusAsync(int id, string? userId, CourseTaskStatus status);
        Task<UserTask> GetUserTaskAsync(int id, string? userId);
        Task<List<UserTask>> GetUserTasksForTasksAsync(string userId, List<int> taskIds);
        Task<List<UserTask>> GetOverdueTasksWithUsersAsync();
        Task CreateUserTaskIfNotExistsAsync(string studentId, int taskId);

    }
}
