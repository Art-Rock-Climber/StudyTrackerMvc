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
    }
}
