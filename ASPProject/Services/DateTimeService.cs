using System.Runtime.CompilerServices;

namespace ASPProject.Services
{
    public class DateTimeService
    {
        //public DateTime GetNow() => DateTime.Now;
        private readonly DateService _dateService;
        private readonly TimeService _timeService;

        public DateTimeService(DateService dateService, TimeService timeService)
        {
            _dateService = dateService;
            _timeService = timeService;
        }
        public DateTime GetNow() => _dateService.GetDate() + _timeService.Gettime().ToTimeSpan();
    }
}
