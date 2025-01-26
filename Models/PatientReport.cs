namespace AppointAid.Models
{
    public class PatientReport
    {
        public int PatientReportId { get; set; } // Primary Key
        public int PatientId { get; set; } // Foreign Key to Patient
        public int? NurseId { get; set; } // Foreign Key to Nurse
        public int? SectorId { get; set; } // Foreign Key to Sector
        public string? PatientSymptoms { get; set; }
        public int? Severity { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public bool IsAppointmentSet { get; set; } = false;

        // Navigation Properties
        public  Patient? Patient { get; set; }
        public  Nurse? Nurse { get; set; }
        public Sector? Sector { get; set; }
    }
}