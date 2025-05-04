using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using stTrackerMVC.Models;
using stTrackerMVC.Services;
using stTrackerMVC.ViewModelBuilders;
using stTrackerMVC.ViewModels;

namespace stTrackerMVC.Controllers
{
    [Route("Course")]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly CoursesVmBuilder _coursesVmBuilder;
        private readonly ILogger<CourseController> _logger;

        public CourseController(
            ICourseService courseService,
            CoursesVmBuilder courseVmBuilder,
            ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _coursesVmBuilder = courseVmBuilder;
            _logger = logger;
        }

        // GET: /Course/
        [HttpGet("")]
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var coursesVm = await _coursesVmBuilder.Build(searchTerm);
            return View(coursesVm);
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
        public async Task<IActionResult> Create([FromForm] CourseVm courseVm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var course = new Course
                    {
                        Id = courseVm.Id,
                        Name = courseVm.Name,
                        Description = courseVm.Description,
                        ProfessorName = courseVm.ProfessorName
                    };
                    await _courseService.AddCourseAsync(course);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                ModelState.AddModelError("", "Ошибка при создании курса");
            }

            return View(courseVm);
        }

        // GET: /Course/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var courseVm = await _coursesVmBuilder.BuildOne(id);
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();

            return View(courseVm);
        }

        // POST: /Course/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] CourseVm courseVm)
        {
            // Защита от подмены ID
            if (id != courseVm.Id) return Forbid();

            if (!ModelState.IsValid) return View(courseVm);

            if (ModelState.IsValid)
            {
                try
                {
                    var course = new Course
                    {
                        Id = courseVm.Id,
                        Name = courseVm.Name,
                        Description = courseVm.Description,
                        ProfessorName = courseVm.ProfessorName
                    };
                    await _courseService.UpdateCourseAsync(course);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating course");
                    ModelState.AddModelError("", "Ошибка при обновлении курса");
                }
            }
            return View(courseVm);
        }

        // GET: /Course/Delete/5
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var courseVm = await _coursesVmBuilder.BuildOne(id);
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();

            return View(courseVm);
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
