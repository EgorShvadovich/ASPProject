using ASPProject.Data;
using ASPProject.Models.Forum.Index;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPProject.Controllers
{
    [Route("api/section")]
    [ApiController]
    public class BackSectionController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public BackSectionController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IEnumerable<ForumSectionViewModel> GetAll()
        {
            int n = 0;
            return _dataContext
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
               });
        }
        [HttpPost]
        public object AddSection([FromBody] SectionData sectionData)
        {
            if (sectionData == null)
            {
                Response.StatusCode = StatusCodes.Status406NotAcceptable;
                return new { Status = "406", Message = "SectionData required" };
            }
            if (String.IsNullOrEmpty(sectionData.Title)
             || String.IsNullOrEmpty(sectionData.Description))
            {
                Response.StatusCode = StatusCodes.Status406NotAcceptable;
                return new { Status = "406", Message = "SectionData could not be empty" };
            }

            Guid id = Guid.NewGuid();

            _dataContext.Sections.Add(new()
            {
                Id = id,
                Title = sectionData.Title,
                Description = sectionData.Description,
                CreateDt = DateTime.Now,
                AuthorId = _dataContext.Users.First().Id,
                DeleteDt = null,
            });
            _dataContext.SaveChanges();

            return new { Status = "200", Message = id.ToString() };
        }
    }
    /* Д.З. Добавление разделов. Вариант API.
     * Реализовать проверку полученных данных (бэкенд) на наличие (уникальность) в
     * БД раздела с заданным заголовком (title). Возращать 409 Conflict
     * Обеспечить обновление (без перегрузки страницы) перечня разделов в случае успешного добавления,
     * в противном случае вывести сообщение об отказе добавления.
     */

    public class SectionData
    {
        public String Title { get; set; } = null!;
        public String Description { get; set; } = null!;
    }
}


/*
 CRUD-полнота - свойство контроллера, означающее что в нем реализованы 
 все методы CRUD
 
 
 */
