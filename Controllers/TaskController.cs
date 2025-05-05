using System;
using System.Collections.Generic;
using System.Linq;
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

        public TaskController(
            ITaskService taskService,
            ICourseService courseService,
            CourseTaskVmBuilder courseTaskVmBuilder,
            ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _courseService = courseService;
            _courseTaskVmBuilder = courseTaskVmBuilder;
            _logger = logger;
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

            var courseTasksVm = await _courseTaskVmBuilder.Build(courseId, statusFilter, sortOrder);
            
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
                        Status = taskVm.Status,
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

            if (ModelState.IsValid)
            {
                try
                {
                    var task = new CourseTask
                    {
                        Id = taskVm.Id,
                        Title = taskVm.Title,
                        Description = taskVm.Description,
                        Deadline = taskVm.Deadline,
                        Status = taskVm.Status,
                        CourseId = taskVm.CourseId
                    };

                    await _taskService.UpdateTaskAsync(task);
                    return RedirectToAction(nameof(ForCourse), new { courseId = task.CourseId });
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
    }
}
