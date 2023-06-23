namespace ASPProject.Services.Validations
{
    public interface IValidationService
    {
        bool IsValid(object model);
        Dictionary<String, String> ErrorMessages(object model);
    }
}
