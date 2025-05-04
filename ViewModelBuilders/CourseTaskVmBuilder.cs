using stTrackerMVC.Models;
using stTrackerMVC.Services;
using stTrackerMVC.ViewModels;
using System.Threading.Tasks;

namespace stTrackerMVC.ViewModelBuilders
{
    public class CourseTaskVmBuilder
    {
        private readonly ICourseService _courseService;
        private readonly ITaskService _taskService;

        public CourseTaskVmBuilder(ICourseService courseService, ITaskService taskService)
        {
            _courseService = courseService;
            _taskService = taskService;
        }

        public async Task<CourseTasksVm?> Build(int? courseId, string? statusFilter)
        {
            IEnumerable<CourseTask> tasks;

            if (courseId.HasValue)
            {
                var course = await _courseService.GetCourseByIdAsync(courseId.Value);
                if (course == null) return null;

                tasks = await _taskService.GetTasksByCourseAsync(courseId.Value);
            }
            else
            {
                tasks = await _taskService.GetAllTasksAsync();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                tasks = tasks.Where(t => t.Status.ToString() == statusFilter);
            }

            return new CourseTasksVm
            {
                Tasks = tasks.Select(t => new CourseTaskVm
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Deadline = t.Deadline,
                    Status = t.Status,
                    CourseName = t.Course?.Name,
                    CourseId = t.CourseId
                }).ToList(),
                CourseId = courseId,
                AvailableStatuses = Enum.GetNames(typeof(CourseTaskStatus))
            };
        }

        public async Task<CourseTaskVm?> BuildOne(int taskId)
        {
            var task = await _taskService.GetTaskAsync(taskId);
            if (task == null) return null;

            return new CourseTaskVm
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Deadline = task.Deadline,
                Status = task.Status,
                CourseName = task.Course?.Name,
                CourseId = task.CourseId
            };
        }
    }
}
