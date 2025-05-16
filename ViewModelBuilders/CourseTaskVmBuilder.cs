using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        public async Task<CourseTasksVm?> Build(int? courseId, string? statusFilter, 
            string? sortOrder = null, string? userId = null)
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

            Dictionary<int, CourseTaskStatus> userStatuses = new();
            if (!string.IsNullOrEmpty(userId))
            {
                // Получаем UserTasks для всех задач одним запросом
                var taskIds = tasks.Select(t => t.Id).ToList();
                var userTasks = await _taskService.GetUserTasksForTasksAsync(userId, taskIds);
                userStatuses = userTasks.ToDictionary(ut => ut.TaskId, ut => ut.Status);
            }

            // Применяем фильтрацию по статусу
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (Enum.TryParse<CourseTaskStatus>(statusFilter, out var status))
                {
                    tasks = tasks.Where(t => userStatuses.TryGetValue(t.Id, out var userStatus)
                                  ? userStatus == status
                                  : status == CourseTaskStatus.NotStarted);
                }
            }

            // Сортировка
            tasks = ApplySorting(tasks, sortOrder);

            return new CourseTasksVm
            {
                Tasks = tasks.Select(t => new CourseTaskVm
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Deadline = t.Deadline,
                    CourseName = t.Course?.Name,
                    CourseId = t.CourseId,
                    Status = userStatuses.TryGetValue(t.Id, out var status)
                    ? status
                    : CourseTaskStatus.NotStarted
                }).ToList(),
                CourseId = courseId,
                AvailableStatuses = Enum.GetNames(typeof(CourseTaskStatus))
            };
        }

        public async Task<CourseTaskVm?> BuildOne(int taskId, string? userId = null)
        {
            // 1. Получаем задание
            var task = await _taskService.GetTaskAsync(taskId);
            if (task == null)
            {
                
                return null;
            }

            // 2. Получаем статус пользователя (если userId передан)
            CourseTaskStatus status = CourseTaskStatus.NotStarted; // Значение по умолчанию

            if (!string.IsNullOrEmpty(userId))
            {
                var userTask = await _taskService.GetUserTaskAsync(taskId, userId);
                if (userTask != null)
                {
                    status = userTask.Status;
                }
            }

            // 3. Создаем ViewModel с проверкой на null
            return new CourseTaskVm
            {
                Id = task.Id,
                Title = task.Title ?? string.Empty,
                Description = task.Description ?? string.Empty,
                Deadline = task.Deadline,
                CourseName = task.Course?.Name ?? "Неизвестный курс",
                CourseId = task.CourseId,
                Status = status
            };
        }

        private static IEnumerable<CourseTask> ApplySorting(IEnumerable<CourseTask> tasks, string? sortOrder)
        {
            return sortOrder switch
            {
                "deadline_asc" => tasks.OrderBy(t => t.Deadline),
                "deadline_desc" => tasks.OrderByDescending(t => t.Deadline),
                _ => tasks
            };
        }
    }
}
