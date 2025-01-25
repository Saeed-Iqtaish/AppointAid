using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointAid.Models
{
    public class Nurse
    {
        public int NurseId { get; set; }
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public required string NationalNumber { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public required string PasswordHash { get; set; }
        public required int MedicalCenterId { get; set; }


        // Navigation properties
        public ICollection<PatientReport> PatientReports { get; set; } = new List<PatientReport>();
        public ICollection<EmergencyResponse> EmergencyResponses { get; set; } = new List<EmergencyResponse>();
        public MedicalCenter? MedicalCenter { get; set; }
    }
}