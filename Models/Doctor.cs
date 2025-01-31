using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointAid.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string NationalNumber { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        public int SectorId { get; set; }

        public string Specialty { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; }

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalTest> MedicalTests { get; set; } = new List<MedicalTest>();
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public Sector? Sector { get; set; }
    }
}