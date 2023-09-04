using ASPProject.Data;
using ASPProject.Models.Forum.Topic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_111.Controllers
{
    [Route("api/theme")]
    [ApiController]
    public class BackThemeController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public BackThemeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IEnumerable<ThemeViewModel>? Get(Guid topicId)
        {
            var topic = _dataContext
               .Topics
               .Include(t => t.Author)
               .FirstOrDefault(t => t.Id == topicId);

            if (topic == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return null;
            }
            return _dataContext
                .Themes
                .Include(t => t.Author)
                .Include(t => t.Comments)
                .Where(t => t.TopicId == topic.Id && t.DeleteDt == null)
                .Select(t => new ThemeViewModel(t));
        }
    }
}