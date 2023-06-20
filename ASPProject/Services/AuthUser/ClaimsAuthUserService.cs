using ASPProject.Data;
using System.Security.Claims;

namespace ASPProject.Services.AuthUser
{
    public class ClaimsAuthUserService : IAuthUserService
    {
        private readonly DataContext _dataContext;


        public ClaimsAuthUserService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Guid? GetUserId(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            Guid userId;
            try
            {   // извлекаем из Claims ID и ...
                userId = Guid.Parse(
                    context.User.Claims.First(
                        c => c.Type == ClaimTypes.Sid
                    ).Value);
            }
            catch (Exception ex)
            {
                return null;
            }
            // ... находим по нему пользователя
            var user = _dataContext.Users.Find(userId);
            return user?.Id;
        }
    }
}
