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
using ASPProject.Models.Forum.Topic;
using ASPProject.Services;
using ASPProject.Models.Forum.Theme;
using ASPProject.Models;

namespace ASPProject.Controllers
{
    public class ForumController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ForumController> _logger;
        private readonly IAuthUserService _authUserService;
        private readonly IValidationService _validationService;
        private readonly IDateService _dateService;

        public ForumController(DataContext dataContext, ILogger<ForumController> logger, IAuthUserService authUserService, IValidationService validationService, IDateService dateService)
        {
            _dataContext = dataContext;
            _logger = logger;
            _authUserService = authUserService;
            _validationService = validationService;
            _dateService = dateService;
        }


        public IActionResult Theme([FromRoute] Guid id)
        {
            var theme = _dataContext
                .Themes
                .Include(t => t.Author)
                .Where(t => t.DeleteDt == null && t.Id == id)
                .FirstOrDefault();
            if (theme == null)
            {
                return NotFound();
            }

            ThemePageModel pageModel = new()
            {
                Theme = new(theme),
                Comments = _dataContext.Comments
                .Include(c => c.Author)
                .OrderBy(c => c.CreateDt)
                .Where(c => c.DeleteDt == null && c.ThemeId == id)
                .Select(c => new CommentViewModel(c))
                .ToList()
            };
            Guid? authUserId = _authUserService.GetUserId(HttpContext);
            if (authUserId != null)
            {
                ViewData["authUser"] = new UserViewModel(_dataContext.Users.Find(authUserId.Value)!);
            }
            return View(pageModel);
        }
        [HttpPost]
        public RedirectToActionResult AddComment([FromForm] CommentFormModel formModel)
        {
            var messages = _validationService.ErrorMessages(formModel);
            foreach (var (key, message) in messages)
            {
                if (message != null)
                {

                    var serializedMessages = JsonSerializer.Serialize(messages);
                    HttpContext.Session.SetString("AddCommentMessage", serializedMessages);
                    return RedirectToAction(nameof(Theme), new { id = formModel.ThemeId });
                }
            }
            Guid? userId = _authUserService.GetUserId(HttpContext);
            if (userId != null)
            {
                _dataContext.Comments.Add(new()
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userId.Value,
                    Content = formModel.Content,
                    ThemeId = formModel.ThemeId,
                    CreateDt = DateTime.Now,
                    ReplyId = formModel.ReplyId
                });
                _dataContext.SaveChanges();
            }
            return RedirectToAction(nameof(Theme), new { id = formModel.ThemeId });
        }
        public IActionResult Topic([FromRoute] Guid id)
        {
            var topic = _dataContext.Topics.Include(t => t.Author).FirstOrDefault(t => t.Id == id);
            if(topic == null)
            {
                return NotFound();
            }
            TopicPageModel model = new()
            {
                Topic = new(topic)
            };
            model.Theme = _dataContext
                .Themes
                .Include(t => t.Author)
                .Include(t => t.Comments)
                .Where(t => t.TopicId == topic.Id && t.DeleteDt == null)
                .Select(t => new ThemeViewModel(t))
                .ToList();

            if (HttpContext.Session.Keys.Contains("AddThemeMessage"))
            {
                model.ErrorMessages =
                    JsonSerializer.Deserialize<Dictionary<String, String?>>(
                        HttpContext.Session.GetString("AddThemeMessage")!);



                HttpContext.Session.Remove("AddThemeMessage");
            }
            return View(model);
        }


        [HttpPost]
        public RedirectToActionResult AddTheme(ThemeFormModel formModel)
        {
            var messages = _validationService.ErrorMessages(formModel);
            foreach (var (key, message) in messages)
            {
                if (message != null)
                {

                    var serializedMessages = JsonSerializer.Serialize(messages);
                    HttpContext.Session.SetString("AddThemeMessage", serializedMessages);
                    return RedirectToAction(nameof(Section), new { id = formModel.TopicId });
                }
            }
            Guid? userId = _authUserService.GetUserId(HttpContext);
            if (userId != null)
            {
                DateTime dt = DateTime.Now;
                Guid themeId = Guid.NewGuid();
                _dataContext.Themes.Add(new()
                {
                    Id = themeId,
                    AuthorId = userId.Value,
                    TopicId = formModel.TopicId,
                    Title = formModel.Title,
                    CreateDt = dt
                });
                
                _dataContext.Comments.Add(new()
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userId.Value,
                    Content = formModel.Content,
                    ThemeId = themeId,
                    CreateDt = dt
                });
                _dataContext.SaveChanges();
            }
            return RedirectToAction(nameof(Topic), new { id = formModel.TopicId });
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
                .Include(s => s.Author)
                .Include(s => s.Rates)
                .Where(s => s.DeleteDt == null)
                .OrderBy(s => s.CreateDt)
                .AsEnumerable().Select(s => new ForumSectionViewModel
                {
                    Id = s.Id.ToString(),
                    Title = s.Title,
                    Description = s.Description,
                    CreateDt = s.CreateDt.ToShortDateString(),
                    ImageUrl = s.ImageUrl == null ? $"/img/section/section{n++}.png" : $"/img/section/{s.ImageUrl}",
                    Author = new(s.Author),
                    Likes = s.Rates.Count(r => r.Raiting > 0),
                    Dislikes = s.Rates.Count(r => r.Raiting < 0)
                }),
            };
            return View(model);
        }


        public IActionResult Section(Guid id)
        {
            var section = _dataContext.Sections
                .Include(s=>s.Author)
                .FirstOrDefault( s => s.Id == id);
            if (section == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return NotFound();
            }
            SectionPageModel sectionViewModel = new()
            {

                Section = new ForumSectionViewModel
                {
                    Id = section.Id.ToString(),
                    Title = section.Title,
                    Description = section.Description,
                    CreateDt = section.CreateDt.ToShortDateString(),
                    Author = new(section.Author)
                }
            };
            if (HttpContext.Session.Keys.Contains("AddTopicMessage"))
            {
                sectionViewModel.ErrorMessages = JsonSerializer.Deserialize<Dictionary<String, String?>>(HttpContext.Session.GetString("AddTopicMessage"));
                HttpContext.Session.Remove("AddTopicMessage");
            }


            sectionViewModel.Topics = _dataContext.Topics
                .Include(t => t.Author)
                .Where(t => t.DeleteDt == null)
                 .OrderByDescending(t => t.CreateDt).AsEnumerable()
                 .Select(t => new TopicViewModel()
                 {
                     Id = t.Id.ToString(),
                     Title = t.Title,
                     Description = t.Description,
                     CreateDt = _dateService.FormatDateTime(t.CreateDt),
                     ImageUrl = "/img/" + t.ImageUrl,
                     Author = new(t.Author)
                 }).ToList();
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
                    return RedirectToAction(nameof(Section), new { id = formModel.SectionId });
                }
            }
            Guid? userId = _authUserService.GetUserId(HttpContext);
            if (userId != null)
            {
                String? nameAvatar = null;
                if (formModel.ImageFile != null)
                {
                    String ext = Path.GetExtension(formModel.ImageFile.FileName);
                    nameAvatar = Guid.NewGuid().ToString() + ext;
                    using FileStream fstream = new("wwwroot/img/" + nameAvatar, FileMode.Create);
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
