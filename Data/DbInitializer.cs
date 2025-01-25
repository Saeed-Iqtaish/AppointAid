using Microsoft.EntityFrameworkCore;
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
                new MedicalCenter { MedicalCenterName = "Green Valley Medical Center", Location = "Suburb" },
                new MedicalCenter { MedicalCenterName = "Riverfront Clinic", Location = "Riverside" }
            };

            context.MedicalCenters.AddRange(medicalCenters);
            context.SaveChanges();

            // Seed Sectors for each Medical Center
            var sectors = new List<Sector>();
            foreach (var medicalCenter in medicalCenters)
            {
                sectors.AddRange(new List<Sector>
                {
                    new Sector { Name = "General Medicine", MedicalCenterId = medicalCenter.MedicalCenterId },
                    new Sector { Name = "Pediatrics", MedicalCenterId = medicalCenter.MedicalCenterId },
                    new Sector { Name = "Cardiology", MedicalCenterId = medicalCenter.MedicalCenterId }
                });
            }

            context.Sectors.AddRange(sectors);
            context.SaveChanges();

            // Seed Doctors for each Sector
            var doctors = new List<Doctor>();
            foreach (var sector in sectors)
            {
                doctors.AddRange(new List<Doctor>
                {
                    new Doctor
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        NationalNumber = $"DR-{sector.SectorId}-1",
                        PasswordHash = "hashedPassword123", // Replace with actual hash
                        SectorId = sector.SectorId,
                        Specialty = $"{sector.Name} Specialist"
                    },
                    new Doctor
                    {
                        FirstName = "Jane",
                        LastName = "Smith",
                        NationalNumber = $"DR-{sector.SectorId}-2",
                        PasswordHash = "hashedPassword123",
                        SectorId = sector.SectorId,
                        Specialty = $"{sector.Name} Specialist"
                    }
                });
            }

            context.Doctors.AddRange(doctors);
            context.SaveChanges();

            // Seed Nurses for each Medical Center
            var nurses = new List<Nurse>();
            foreach (var medicalCenter in medicalCenters)
            {
                nurses.AddRange(new List<Nurse>
                {
                    new Nurse
                    {
                        FirstName = "Alice",
                        LastName = "Brown",
                        NationalNumber = $"NR-{medicalCenter.MedicalCenterId}-1",
                        PasswordHash = "hashedPassword123",
                        MedicalCenterId = medicalCenter.MedicalCenterId
                    },
                    new Nurse
                    {
                        FirstName = "Bob",
                        LastName = "White",
                        NationalNumber = $"NR-{medicalCenter.MedicalCenterId}-2",
                        PasswordHash = "hashedPassword123",
                        MedicalCenterId = medicalCenter.MedicalCenterId
                    }
                });
            }

            context.Nurses.AddRange(nurses);
            context.SaveChanges();
        }
    }
}