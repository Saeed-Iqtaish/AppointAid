namespace AppointAid.Models
{

    public class MedicalTest
    {
        public int MedicalTestId { get; set; }
        public string TestName { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public TimeSpan? ScheduledTime { get; set; }
        public string? Results { get; set; }
        public required string Reason { get; set; }
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}
