using ASPProject.Data;
using ASPProject.Models.Forum.Index;
using ASPProject.Services.AuthUser;
using ASPProject.Services.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ASPProject.Models.Forum.Section;

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
            /*var validationMessages = HttpContext.Session.GetString("ValidationMessages");
            if (!string.IsNullOrEmpty(validationMessages))
            {
                var deserializedMessages = JsonSerializer.Deserialize<Dictionary<string, string>>(validationMessages);
                ViewData["ValidationMessages"] = deserializedMessages;
                HttpContext.Session.Remove("ValidationMessages");
                
            }*/
            int n = 0;
            ForumIndexModel model = new()
            {
                Sections = _dataContext
                .Sections
                .Include(s=>s.Author)
                .Where(s => s.DeleteDt == null)
                .OrderBy(s=>s.CreateDt)
                .AsEnumerable().Select(s => new ForumSectionViewModel
                {
                    Id = s.Id.ToString(),
                    Title = s.Title,
                    Description = s.Description,
                    CreateDt = s.CreateDt.ToShortDateString(),
                    ImageUrl = s.ImageUrl == null ? $"/img/section/section{n++}.png" : $"/img/section/{s.ImageUrl}",
                    Author = new(s.Author),
                }),
            };
            return View(model);
        }


        public ViewResult Section(Guid id)
        {
            SectionViewModel sectionViewModel = new()
            {
                SectionId = id.ToString()
            };
           if(HttpContext.Session.Keys.Contains("AddTopicMessage"))
            {
                sectionViewModel.ErrorMessages = JsonSerializer.Deserialize<Dictionary<String, String?>>(HttpContext.Session.GetString("AddTopicMessage"));
                HttpContext.Session.Remove("AddTopicMessage");
            }
            return View(sectionViewModel);
        }

        [HttpPost]
        public RedirectToActionResult AddTopic(TopicFormModel formModel)
        {
            var messages = _validationService.ErrorMessages(formModel);
            foreach (var (key, message) in messages)
            {
                if (message != null)
                {

                    var serializedMessages = JsonSerializer.Serialize(messages);
                    HttpContext.Session.SetString("AddTopicMessage", serializedMessages);
                    return RedirectToAction(nameof(Section),new {id = formModel.SectionId});
                }
            }
            Guid? userId = _authUserService.GetUserId(HttpContext);
            if (userId != null)
            {
                String? nameAvatar = null;
                if(formModel.ImageFile != null)
                {
                    String ext = Path.GetExtension(formModel.ImageFile.FileName);
                    nameAvatar = Guid.NewGuid().ToString() + ext;
                    using FileStream fstream = new ("wwwroot/img/" + nameAvatar, FileMode.Create);
                    formModel.ImageFile.CopyTo(fstream);
                }
                _dataContext.Topics.Add(new()
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userId.Value,
                    SectionId = formModel.SectionId,
                    Title = formModel.Title,
                    Description = formModel.Description,
                    CreateDt = DateTime.Now,
                    ImageUrl = nameAvatar 
                });
                _dataContext.SaveChanges();
            }
            return RedirectToAction(nameof(Section));
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
            String? nameAvatar = null;
            if (model.ImageFile != null)
            {
                String ext = Path.GetExtension(model.ImageFile.FileName);
                nameAvatar = Guid.NewGuid().ToString() + ext;
                using FileStream fstream = new("wwwroot/img/section/" + nameAvatar, FileMode.Create);
                model.ImageFile.CopyTo(fstream);
            }
            if (userId != null)
            {
                _dataContext.Sections.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    Description = model.Description,
                    CreateDt = DateTime.Now,
                    ImageUrl = nameAvatar,
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
