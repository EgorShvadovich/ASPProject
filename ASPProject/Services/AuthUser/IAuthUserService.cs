namespace ASPProject.Services.AuthUser
{
    public interface IAuthUserService
    {
        Guid? GetUserId(HttpContext context);
    }
}
