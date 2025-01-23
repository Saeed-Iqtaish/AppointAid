using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointAid.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        [Required]
        public string NationalNumber { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalTest> MedicalTests { get; set; } = new List<MedicalTest>();
        public ICollection<PatientReport> PatientReports { get; set; } = new List<PatientReport>();
        public ICollection<EmergencyResponse> EmergencyResponses { get; set; } = new List<EmergencyResponse>();
    }
}