using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Data;
using stTrackerMVC.Models;
using stTrackerMVC.Services;
using stTrackerMVC.ViewModelBuilders;
using stTrackerMVC.ViewModels;

namespace stTrackerMVC.Controllers
{
    [Route("Task")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ICourseService _courseService;
        private readonly CourseTaskVmBuilder _courseTaskVmBuilder;
        private readonly ILogger<TaskController> _logger;
        private readonly ReportGeneratorService _reportGenerator;

        public TaskController(
            ITaskService taskService,
            ICourseService courseService,
            CourseTaskVmBuilder courseTaskVmBuilder,
            ILogger<TaskController> logger,
            ReportGeneratorService reportGenerator)
        {
            _taskService = taskService;
            _courseService = courseService;
            _courseTaskVmBuilder = courseTaskVmBuilder;
            _logger = logger;
            _reportGenerator = reportGenerator;
        }

        // GET: Task/ForCourse/5
        [HttpGet("ForCourse/{courseId:int}")]
        public async Task<IActionResult> ForCourse(
            int courseId, 
            string? statusFilter,
            string? sortOrder)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            // Получаем ID текущего пользователя
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courseTasksVm = await _courseTaskVmBuilder.Build(courseId, statusFilter, sortOrder, userId);
            
            ViewBag.Course = course;
            return View(courseTasksVm);
        }

        // GET: Task/Create?courseId=5
        [HttpGet("Create/{courseId:int}")]
        public async Task<IActionResult> Create(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            ViewBag.Course = course;
            return View(new CourseTaskVm
            {
                CourseId = courseId,
                Deadline = DateTime.Now.AddDays(7)
            });
        }

        [HttpPost("Create/{courseId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int courseId, [FromForm] CourseTaskVm taskVm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var task = new CourseTask
                    {
                        Title = taskVm.Title,
                        Description = taskVm.Description,
                        Deadline = taskVm.Deadline,
                        CourseId = courseId
                    };

                    await _taskService.CreateTaskAsync(task);
                    return RedirectToAction(nameof(ForCourse), new { courseId = task.CourseId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating task");
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Если ошибка, возвращаем на форму с введёнными данными
            ViewBag.Course = await _courseService.GetCourseByIdAsync(courseId);
            return View(taskVm);
        }

        // GET: Task/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var taskVm = await _courseTaskVmBuilder.BuildOne(id);
            if (taskVm == null) return NotFound();

            var course = await _courseService.GetCourseByIdAsync(taskVm.CourseId);
            if (course == null) return NotFound();

            ViewBag.Course = course;
            return View(taskVm);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] CourseTaskVm taskVm)
        {
            if (id != taskVm.Id) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Пользователь не аутентифицирован");
                return View(taskVm);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Для администратора - обновляем данные задания
                    if (User.IsInRole("Admin"))
                    {
                        var task = new CourseTask
                        {
                            Id = taskVm.Id,
                            Title = taskVm.Title,
                            Description = taskVm.Description,
                            Deadline = taskVm.Deadline,
                            CourseId = taskVm.CourseId
                        };
                        await _taskService.UpdateTaskAsync(task);
                    }

                    // Для всех пользователей - обновляем их персональный статус
                    await _taskService.UpdateUserTaskStatusAsync(taskVm.Id, userId, taskVm.Status);
                    return RedirectToAction(nameof(ForCourse), new { courseId = taskVm.CourseId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            var course = await _courseService.GetCourseByIdAsync(taskVm.CourseId);
            ViewBag.Course = course;
            return View(taskVm);
        }

        // POST: Task/UpdateStatus/5?status=InProgress
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int taskId, CourseTaskStatus status)
        {
            try
            {
                await _taskService.UpdateTaskStatusAsync(taskId, status);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        // GET: Task/Delete/5
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var taskVm = await _courseTaskVmBuilder.BuildOne(id);
            if (taskVm == null) return NotFound();

            var course = await _courseService.GetCourseByIdAsync(taskVm.CourseId);
            if (course == null) return NotFound();

            ViewBag.Course = course;
            return View(taskVm);
        }

        [HttpPost("Delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var task = await _taskService.GetTaskAsync(id);
                if (task == null) return NotFound();

                var courseId = task.CourseId;
                await _taskService.DeleteTaskAsync(id);

                return RedirectToAction(nameof(ForCourse), new { courseId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        [HttpGet("AllTasks")]
        public async Task<IActionResult> AllTasks()
        {
            var tasksVm = await _courseTaskVmBuilder.Build(null, null);
            return View(tasksVm);
        }

        [HttpGet("Task/ExportCourseTasks/{courseId}")]
        public async Task<IActionResult> ExportCourseTasks(int courseId, string format)
        {
            // Получаем задачи курса
            var tasks = await _taskService.GetTasksByCourseAsync(courseId);

            // Получаем UserTasks для текущего пользователя
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var taskIds = tasks.Select(t => t.Id).ToList();
            var userTasks = await _taskService.GetUserTasksForTasksAsync(currentUserId, taskIds);

            byte[] fileContents;
            string contentType;
            string fileDownloadName;

            if (format.ToLower() == "xlsx")
            {
                fileContents = _reportGenerator.GenerateExcelReport(tasks, currentUserId, userTasks);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileDownloadName = $"Задания_курса_{courseId}_{DateTime.Now:yyyyMMdd}.xlsx";
            }
            else if (format.ToLower() == "docx")
            {
                fileContents = _reportGenerator.GenerateWordReport(tasks, currentUserId, userTasks);
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                fileDownloadName = $"Задания_курса_{courseId}_{DateTime.Now:yyyyMMdd}.docx";
            }
            else
            {
                return BadRequest("Неподдерживаемый формат");
            }

            return File(fileContents, contentType, fileDownloadName);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Task/ExportOverdueTasks")]
        public async Task<IActionResult> ExportOverdueTasks(string format)
        {
            // Получаем все просроченные задания с пользователями
            var userTasks = await _taskService.GetOverdueTasksWithUsersAsync();

            // Группируем задачи для отчета
            var tasks = userTasks.Select(ut => ut.Task).Distinct().ToList();

            byte[] fileContents;
            string contentType;
            string fileDownloadName;

            if (format.ToLower() == "xlsx")
            {
                fileContents = _reportGenerator.GenerateOverdueExcelReport(tasks, userTasks);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileDownloadName = $"Просроченные_задания_{DateTime.Now:yyyyMMdd}.xlsx";
            }
            else
            {
                return BadRequest("Неподдерживаемый формат");
            }

            return File(fileContents, contentType, fileDownloadName);
        }
    }
}
