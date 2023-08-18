using Microsoft.AspNetCore.Mvc;

namespace ASPProject.Controllers
{
    public class FrontController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
