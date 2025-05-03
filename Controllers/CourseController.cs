using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using stTrackerMVC.Data;
using stTrackerMVC.Models;
using stTrackerMVC.Services;

namespace stTrackerMVC.Controllers
{
    [Route("Course")]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(
            ICourseService courseService,
            ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        // GET: /Course/
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetCoursesAsync();
            return View(courses);
        }

        // GET: /Course/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Course/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ProfessorName")] Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _courseService.AddCourseAsync(course);
                    return RedirectToAction(nameof(CourseController.Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                ModelState.AddModelError("", "Ошибка при создании курса");
            }

            return View(course);
        }

        // GET: /Course/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return View(course);
        }

        // POST: /Course/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ProfessorName")] Course course)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _courseService.UpdateCourseAsync(course);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating course");
                    ModelState.AddModelError("", "Ошибка при обновлении курса");
                }
            }
            return View(course);
        }

        // GET: /Course/Delete/5
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return View(course);
        }

        // POST: /Course/Delete/5
        [HttpPost("Delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка удаления курса {id}");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}
