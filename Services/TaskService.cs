﻿using Microsoft.EntityFrameworkCore;
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
            //if (task.Deadline < DateTime.Today)
            //    throw new ArgumentException("Дедлайн не может быть в прошлом");

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
                            t.Deadline >= DateTime.Now)
                .OrderBy(t => t.Deadline)
                .ToList();
        }

        public async Task<IEnumerable<CourseTask>> GetAllTasksAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task UpdateUserTaskStatusAsync(int id, string? userId, CourseTaskStatus status)
        {
            await _repository.UpdateUserTaskStatusAsync(id, userId, status);
        }

        public async Task<UserTask> GetUserTaskAsync(int id, string? userId)
        {
            return await _repository.GetUserTaskAsync(id, userId);
        }

        public async Task<List<UserTask>> GetUserTasksForTasksAsync(string userId, List<int> taskIds)
        {
            return await _repository.GetUserTasksForTasksAsync(userId, taskIds);
        }

        public async Task<List<UserTask>> GetOverdueTasksWithUsersAsync()
        {
            return await _repository.GetOverdueTasksWithUsersAsync(DateTime.Now);
        }

        public async Task CreateUserTaskIfNotExistsAsync(string studentId, int taskId)
        {
            var exists = await _repository.UserTaskExistsAsync(studentId, taskId);
            if (!exists)
            {
                var userTask = new UserTask
                {
                    UserId = studentId,
                    TaskId = taskId,
                    Status = CourseTaskStatus.NotStarted,
                };
                await _repository.AddUserTaskAsync(userTask);
            }
        }
    }
}
