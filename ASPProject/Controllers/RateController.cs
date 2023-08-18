using ASPProject.Entities;
using Microsoft.AspNetCore.Http;
using ASPProject.Data;
using ASPProject.Services.AuthUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPProject.Controllers
{
    [Route("api/rate")]
    [ApiController]
    public class RateController : ControllerBase
    {

        private readonly DataContext _dataContext;
        private readonly IAuthUserService _authService;

        public RateController(DataContext dataContext, IAuthUserService authService)
        {
            _dataContext = dataContext;
            _authService = authService;
        }
        [HttpGet]
        public object DoGet()
        {
            return new { message = "Hello from GET" };
        }
        [HttpPost]
        public object DoPost([FromQuery] Guid itemId, [FromQuery] int rateValue)
        {
           
            Guid? userId = _authService.GetUserId(HttpContext);
            if (userId == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return "Authorization required";
            }
            if (itemId == default(Guid))
            {
                Response.StatusCode = StatusCodes.Status406NotAcceptable;
                return "Invalid parameter: itemId";
            }
            if (rateValue == default(int))
            {
                Response.StatusCode = StatusCodes.Status406NotAcceptable;
                return "Invalid parameter: rateValue";
            }
            var rate = new Entities.Rate()
            {
                ItemId = itemId,
                UserId = userId.Value,
                Moment = DateTime.Now,
                Raiting = rateValue
            };

            _dataContext.Rates.Add(rate);
            try
            {
                _dataContext.SaveChanges();
            }
            catch
            {
                Response.StatusCode = StatusCodes.Status406NotAcceptable;
                return "Invalid parameters: already exists";
            }

            Response.StatusCode = StatusCodes.Status201Created;
            return rate;
        }
        [HttpPut]
        public object DoPut([FromBody] dynamic body)
        {
            return body;
        }
        [HttpDelete]
        public object DoDelete([FromQuery] String id)
        {
            return new { id };
        }
        public object DoDefault()
        {
            return this.Request.Method switch
            {
                "LINK" => DoLink(),
                _ => new { method = this.Request.Method }
            };

        }
        private object DoLink()
        {
            return new { link = true };
        }
    }
}
