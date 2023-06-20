using ASPProject.Data;
using ASPProject.Models.Forum.Index;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPProject.Controllers
{
    public class ForumController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ForumController> _logger;

        public ForumController(DataContext dataContext, ILogger<ForumController> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public RedirectToActionResult AddSection(ForumSectionFormModel model)
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                Guid userId;
                try
                {   // извлекаем из Claims ID и ...
                    userId = Guid.Parse(
                        HttpContext.User.Claims.First(
                            c => c.Type == ClaimTypes.Sid
                        ).Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError("UpdateEmail exception {ex}", ex.Message);
                    return RedirectToAction(nameof(Index));
                }
                // ... находим по нему пользователя
                var user = _dataContext.Users.Find(userId);
                if (user != null)
                {
                    _dataContext.Sections.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Title = model.Title,
                        Description = model.Description,
                        CreateDt = DateTime.Now,
                        ImageUrl = null,
                        DeleteDt = null,
                        AuthorId = userId
                    });
                    _dataContext.SaveChanges();
                    _logger.LogInformation("Add OK");
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
