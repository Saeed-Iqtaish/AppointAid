namespace AppointAid.Models
{
    public class EmergencyResponse
    {
        public int EmergencyResponseId { get; set; }
        public int PatientId { get; set; }
        public int? NurseId { get; set; }
        public string EmergencyType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool? IsCritical { get; set; } // Null indicates pending review
        public string? NurseInstructions { get; set; } // Used for non-critical cases
        public DateTime? ResponseTime { get; set; } // Estimated team arrival for critical cases

        // Navigation Properties
        public Patient? Patient { get; set; }
        public Nurse? Nurse { get; set; }
    }
}