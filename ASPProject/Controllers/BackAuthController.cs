using ASPProject.Data;
using ASPProject.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace ASPProject.Controllers
{
    [Route("api/auth")]
    public class BackAuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IHashService _hashService;

        public BackAuthController(DataContext dataContext, IHashService hashService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
        }

        [HttpPost]
        public object DoPost([FromBody] AuthData authData)
        {
            var user = _dataContext.Users.FirstOrDefault(
                u => u.Login == authData.Login
                && u.PasswordHash == _hashService.GetHash(authData.Password)
     );
            // сохранение информации об аутентификации в сессии
            if (user != null)
            {
                Entities.Token token = new()
                {
                    Id = _hashService.GetHash(Guid.NewGuid().ToString()),
                    Expires = DateTime.Now.AddDays(1)
                };
                _dataContext.Tokens.Add(token);
                _dataContext.SaveChanges();
                return token;
            }
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return new { Id = ""};
           
        }
    }

    public class AuthData
    {
        public String Login { get; set; } = null!;
        public String Password { get; set; } = null!;
    }
}
