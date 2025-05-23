using stTrackerMVC.Models;

namespace stTrackerMVC.Repositories
{
    public interface ITaskRepository
    {
        Task<CourseTask> GetByIdAsync(int id);
        Task<List<CourseTask>> GetByCourseIdAsync(int courseId, CourseTaskStatus? status = null);
        Task CreateAsync(CourseTask task);
        Task UpdateAsync(CourseTask task);
        Task DeleteAsync(int id);
        Task UpdateStatusAsync(int taskId, CourseTaskStatus status);
        Task<List<CourseTask>> GetAllAsync();
        Task UpdateUserTaskStatusAsync(int taskId, string? userId, CourseTaskStatus status);
        Task<UserTask?> GetUserTaskAsync(int taskId, string? userId);
        Task<List<UserTask>> GetUserTasksForTasksAsync(string userId, List<int> taskIds);
        Task<List<UserTask>> GetOverdueTasksWithUsersAsync(DateTime currentDate);
        Task<bool> UserTaskExistsAsync(string studentId, int taskId);
        Task AddUserTaskAsync(UserTask userTask);
    }
}
