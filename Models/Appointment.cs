namespace AppointAid.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int PatientReportId { get; set; }
        public int TimeSlotId { get; set; }
        public string Status { get; set; } = string.Empty;

        // Navigation Properties
        public  Patient? Patient { get; set; }
        public  Doctor? Doctor { get; set; }
        public  PatientReport? PatientReport { get; set; }
        public  TimeSlot? TimeSlot { get; set; }
    }
}