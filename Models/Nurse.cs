using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointAid.Models
{
    public class Nurse
    {
        public int NurseId { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string NationalNumber { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        [Required]
        public string PasswordHash { get; set; }


        // Navigation properties
        public ICollection<PatientReport> PatientReports { get; set; } = new List<PatientReport>();
        public ICollection<EmergencyResponse> EmergencyResponses { get; set; } = new List<EmergencyResponse>();
    }
}