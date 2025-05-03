using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using stTrackerMVC.ViewModelBuilders;
using stTrackerMVC.ViewModels;

namespace stTrackerMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CoursesVmBuilder _coursesVmBuilder;

        public HomeController(ILogger<HomeController> logger, CoursesVmBuilder coursesVmBuilder)
        {
            _logger = logger;
            _coursesVmBuilder = coursesVmBuilder;
        }

        public async Task<IActionResult> Index()
        {
            var coursesVm = await _coursesVmBuilder.Build();
            return View(coursesVm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
