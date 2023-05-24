using ASPProject.Models;
using ASPProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASPProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DateService _dateService;
        private readonly TimeService _timeService;
        private readonly DateTimeService _dateTimeService;
        public HomeController(ILogger<HomeController> logger,DateService dateService, TimeService timeService, DateTimeService dateTimeService)
        {
            _logger = logger;
            _dateService = dateService;
            _timeService = timeService;
            _dateTimeService = dateTimeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Intro()
        {
            return View();
        }
        public IActionResult Razor()
        {
            return View();
        }
        public IActionResult Services()
        { 
            ViewData["date"] = _dateService.GetDate();
            ViewData["time"] = _timeService.Gettime();
            ViewData["datetime"] = _dateTimeService.GetNow();

            ViewData["date-hash"] = _dateService.GetHashCode();
            ViewData["time-hash"] = _timeService.GetHashCode();
            ViewData["datetime-hash"] = _dateTimeService.GetHashCode();
            ViewData["ctrl-hash"] = this.GetHashCode();
            return View();
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