namespace ASPProject.Services
{
    public class DateService : IDateService
    {
        public DateTime GetDate() => DateTime.Today;
        public String FormatDateTime(DateTime dateTime)
        {
            var currentDate = DateTime.Now.Date;

            if (dateTime.Date == currentDate)
            {
                return dateTime.ToString("HH:mm:ss");
            }

            if (dateTime.Date == currentDate.AddDays(-1))
            {
                return "вчера " + dateTime.ToString("HH:mm");
            }

            if (currentDate.Subtract(dateTime.Date).TotalDays < 10)
            {
                return $"{currentDate.Subtract(dateTime.Date).Days} дней назад";
            }

            return dateTime.ToString("dd.MM.yyyy");
        }
    }
}
