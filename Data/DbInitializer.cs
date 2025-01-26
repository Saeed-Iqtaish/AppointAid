using AppointAid.Models;
using System.Collections.Generic;

namespace AppointAid.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Check if the database is already seeded
            if (context.MedicalCenters.Any() || context.Nurses.Any() || context.Doctors.Any() || context.Sectors.Any())
            {
                return; // Database has been seeded
            }

            // Seed Medical Centers
            var medicalCenters = new List<MedicalCenter>
            {
                new MedicalCenter { MedicalCenterName = "City Hospital", Location = "Downtown" },
            };

            context.MedicalCenters.AddRange(medicalCenters);
            context.SaveChanges();

            // Seed Sectors for each Medical Center
            var sectors = new List<Sector>();
            foreach (var medicalCenter in medicalCenters)
            {
                sectors.Add(new Sector
                {
                    Name = "General Medicine",
                    MedicalCenterId = medicalCenter.MedicalCenterId
                });
            }

            context.Sectors.AddRange(sectors);
            context.SaveChanges();

            // Seed Doctors for each Sector
            var doctors = new List<Doctor>();
            foreach (var sector in sectors)
            {
                doctors.Add(new Doctor
                {
                    FirstName = "John",
                    LastName = "Doe",
                    NationalNumber = $"DR-{sector.SectorId}-1",
                    PasswordHash = "Doctor123", // Plain text password
                    SectorId = sector.SectorId,
                    Specialty = $"{sector.Name} Specialist"
                });
            }

            context.Doctors.AddRange(doctors);
            context.SaveChanges();

            // Seed Nurses for each Medical Center
            var nurses = new List<Nurse>();
            foreach (var medicalCenter in medicalCenters)
            {
                nurses.Add(new Nurse
                {
                    FirstName = "Alice",
                    LastName = "Brown",
                    NationalNumber = $"NR-{medicalCenter.MedicalCenterId}-1",
                    PasswordHash = "Nurse123", // Plain text password
                    MedicalCenterId = medicalCenter.MedicalCenterId
                });
            }

            context.Nurses.AddRange(nurses);
            context.SaveChanges();
        }
    }
}