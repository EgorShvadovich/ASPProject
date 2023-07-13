namespace ASPProject.Services
{
    public interface IDateService
    {
        DateTime GetDate();
        String FormatDateTime(DateTime dateTime);
    }
}
