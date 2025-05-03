using stTrackerMVC.Services;
using stTrackerMVC.ViewModels;

namespace stTrackerMVC.ViewModelBuilders
{
    public class CourseTaskVmBuilder
    {
        private readonly ICourseService _courseService;

        public CourseTaskVmBuilder(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<TasksVm> Build(int courseId)
        {
            var course = await _courseService.GetCourseWithTasksAsync(courseId);

            return new TasksVm
            {
                CourseId = course.Id,
                CourseName = course.Name,
                Tasks = course.Tasks.Select(t => new TaskItemVm
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Deadline = t.Deadline.ToString("dd.MM.yyyy")
                }).ToList()
            };
        }
    }
}
