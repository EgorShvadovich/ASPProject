using ASPProject.Data;
using ASPProject.Models.Forum.Index;
using ASPProject.Services.AuthUser;
using ASPProject.Services.Validations;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPProject.Controllers
{
    public class ForumController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ForumController> _logger;
        private readonly IAuthUserService _authUserService;
        private readonly IValidationService _validationService;

        public ForumController(DataContext dataContext, ILogger<ForumController> logger, IAuthUserService authUserService, IValidationService validationService)
        {
            _dataContext = dataContext;
            _logger = logger;
            _authUserService = authUserService;
            _validationService = validationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public RedirectToActionResult AddSection(ForumSectionFormModel model)
        {
            _validationService.IsValid(model);

            Guid? userId = _authUserService.GetUserId(HttpContext);
            if(userId != null)
            {
                _dataContext.Sections.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    Description = model.Description,
                    CreateDt = DateTime.Now,
                    ImageUrl = null,
                    DeleteDt = null,
                    AuthorId = userId.Value
                });
                _dataContext.SaveChanges();
                _logger.LogInformation("Add OK");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
