namespace AppointAid.ViewModels
{
    public class WeeklyScheduleViewModel
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }

        public List<ScheduleDay> Days { get; set; } = new List<ScheduleDay>();
    }

    public class ScheduleDay
    {
        public string Day { get; set; }
        public bool IsAvailable { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
