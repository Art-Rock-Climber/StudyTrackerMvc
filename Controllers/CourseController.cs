using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using stTrackerMVC.Models;
using stTrackerMVC.Services;
using stTrackerMVC.ViewModelBuilders;
using stTrackerMVC.ViewModels;

namespace stTrackerMVC.Controllers
{
    [Route("Course")]
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly CoursesVmBuilder _coursesVmBuilder;
        private readonly ILogger<CourseController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public CourseController(
            ICourseService courseService,
            CoursesVmBuilder courseVmBuilder,
            ILogger<CourseController> logger,
            UserManager<AppUser> userManager)
        {
            _courseService = courseService;
            _coursesVmBuilder = courseVmBuilder;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: /Course/
        [HttpGet("")]
        public async Task<IActionResult> Index(string? searchTerm)
        {
            if (User.IsInRole("Admin"))
            {
                // Админ видит все курсы
                var coursesVm = await _coursesVmBuilder.Build(searchTerm);
                return View(coursesVm);
            }
            else
            {
                // Студент видит только свои курсы
                var userId = _userManager.GetUserId(User);
                var coursesVm = await _coursesVmBuilder.BuildForStudent(userId, searchTerm);
                return View(coursesVm);
            }
        }

        //// GET: /Course/Details/5
        //[HttpGet("Details/{id:int}")]
        //public async Task<IActionResult> Details(int id)
        //{
        //    var courseVm = await _coursesVmBuilder.BuildOne(id);

        //    // Для студентов проверяем, назначен ли курс
        //    if (!User.IsInRole("Admin"))
        //    {
        //        var userId = _userManager.GetUserId(User);
        //        var isAssigned = await _courseService.IsStudentAssignedToCourseAsync(userId, id);

        //        if (!isAssigned) return Forbid();
        //    }

        //    return View(courseVm);
        //}

        // GET: /Course/Create
        [HttpGet("Create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Course/Create
        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var courseVm = await _coursesVmBuilder.BuildOne(id);
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();

            return View(courseVm);
        }

        // POST: /Course/Edit/5
        [HttpPost("Edit/{id:int}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var courseVm = await _coursesVmBuilder.BuildOne(id);
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();

            return View(courseVm);
        }

        // POST: /Course/Delete/5
        [HttpPost("Delete/{id:int}"), ActionName("Delete")]
        [Authorize(Roles = "Admin")]
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

        // GET: /Course/AssignStudents/5
        [HttpGet("AssignStudents/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignStudents(int id)
        {
            var viewModel = await _coursesVmBuilder.BuildAssignStudentsViewModel(id);
            return View(viewModel);
        }

        // POST: /Course/AssignStudents
        [HttpPost("AssignStudents/{id:int}"), ActionName("AssignStudents")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudents([FromForm] AssignStudentsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = await _coursesVmBuilder.BuildAssignStudentsViewModel(model.CourseId);
                return View(viewModel);
            }

            try
            {
                // Получаем выбранные ID студентов из AvailableStudents
                var selectedIds = model.AvailableStudents
                    .Where(s => s.IsSelected)
                    .Select(s => s.Id)
                    .ToList();

                await _courseService.AssignStudentsToCourseAsync(model.CourseId, selectedIds);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning students to course");
                ModelState.AddModelError("", "Ошибка при назначении студентов на курс");

                var viewModel = await _coursesVmBuilder.BuildAssignStudentsViewModel(model.CourseId);
                return View(viewModel);
            }
        }
    }
}
