using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Data;
using stTrackerMVC.Models;
using stTrackerMVC.Services;

namespace stTrackerMVC.Controllers
{
    [Route("Task")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ICourseService _courseService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(
            ITaskService taskService,
            ICourseService courseService,
            ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _courseService = courseService;
            _logger = logger;
        }

        // GET: Task/ForCourse/5
        [HttpGet("ForCourse/{courseId:int}")]
        public async Task<IActionResult> ForCourse(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            ViewBag.Course = course;
            var tasks = await _taskService.GetTasksByCourseAsync(courseId);
            return View(tasks);
        }

        // GET: Task/Create?courseId=5
        [HttpGet("Create/{courseId:int}")]
        public async Task<IActionResult> Create(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();

            ViewBag.Course = course;
            return View(new CourseTask
            {
                CourseId = courseId,
                Deadline = DateTime.Now.AddDays(7)
            });
        }

        [HttpPost("Create/{courseId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Deadline,Status,CourseId")] CourseTask task)
        {
            if (ModelState.IsValid)
            {
                try
                {
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
            ViewBag.Course = await _courseService.GetCourseByIdAsync(task.CourseId);
            return View(task);
        }

        // GET: Task/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskService.GetTaskAsync(id);
            if (task == null) return NotFound();

            return View(task);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseTask task)
        {
            if (id != task.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _taskService.UpdateTaskAsync(task);
                    return RedirectToAction(nameof(ForCourse), new { courseId = task.CourseId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(task);
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
            var task = await _taskService.GetTaskAsync(id);
            if (task == null) return NotFound();
            
            ViewBag.Course = await _courseService.GetCourseByIdAsync(task.CourseId);
            return View(task);
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

                return RedirectToAction("ForCourse", new { courseId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                return RedirectToAction("Delete", new { id });
            }
        }

        [HttpGet("AllTasks")]
        public async Task<IActionResult> AllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return View(tasks);
        }
    }
}
