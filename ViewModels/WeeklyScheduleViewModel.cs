using System.ComponentModel.DataAnnotations;

namespace AppointAid.ViewModels
{
    public class WeeklyScheduleViewModel
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public List<ScheduleDay> Days { get; set; } = new List<ScheduleDay>();
    }

    public class ScheduleDay
    {
        public string Day { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }
    }
}