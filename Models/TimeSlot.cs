namespace AppointAid.Models
{

    public class TimeSlot
    {
        public int TimeSlotId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool Availability { get; set; }
        public ICollection<DoctorTimeSlot>? DoctorTimeSlots { get; set; } = new List<DoctorTimeSlot>();
    }
}