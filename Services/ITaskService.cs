using stTrackerMVC.Models;

namespace stTrackerMVC.Services
{
    public interface ITaskService
    {
        Task<CourseTask> GetTaskAsync(int id);
        Task<List<CourseTask>> GetTasksByCourseAsync(int courseId);
        Task<IEnumerable<CourseTask>> GetAllTasksAsync();
        Task CreateTaskAsync(CourseTask task);
        Task UpdateTaskAsync(CourseTask task);
        Task DeleteTaskAsync(int id);
        Task UpdateTaskStatusAsync(int taskId, CourseTaskStatus status);
        Task<List<CourseTask>> GetUpcomingDeadlinesAsync(int days = 7);
    }
}
