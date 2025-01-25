namespace AppointAid.Models
{

    public class MedicalTest
    {
        public int MedicalTestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public int? DoctorId { get; set; } 
        public bool? IsApproved { get; set; }
        public DateTime? Time { get; set; }
        public string? Results { get; set; }
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
