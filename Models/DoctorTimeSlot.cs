namespace AppointAid.Models
{
    public class DoctorTimeSlot
    {
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public int? TimeSlotId { get; set; }
        public TimeSlot? TimeSlot { get; set; }
    }
}