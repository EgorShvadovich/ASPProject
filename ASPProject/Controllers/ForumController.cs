using ASPProject.Data;
using ASPProject.Models.Forum.Index;
using ASPProject.Services.AuthUser;
using ASPProject.Services.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

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
            var validationMessages = HttpContext.Session.GetString("ValidationMessages");
            if (!string.IsNullOrEmpty(validationMessages))
            {
                var deserializedMessages = JsonSerializer.Deserialize<Dictionary<string, string>>(validationMessages);
                ViewData["ValidationMessages"] = deserializedMessages;
                HttpContext.Session.Remove("ValidationMessages");
            }

            return View();
        }

        [HttpPost]
        public RedirectToActionResult AddSection(ForumSectionFormModel model)
        {
            var messages = _validationService.ErrorMessages(model);
            foreach (var (key, message) in messages)
            {
                if (message != null)
                {
                    var serializedMessages = JsonSerializer.Serialize(messages);
                    HttpContext.Session.SetString("ValidationMessages", serializedMessages);
                    return RedirectToAction(nameof(Index));
                }
            }

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
