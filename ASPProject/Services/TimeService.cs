namespace ASPProject.Services
{
    public class TimeService
    {
        public TimeOnly Gettime() => TimeOnly.FromDateTime(DateTime.Now);
    }
}
