using AppointAid.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointAid.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalTest> MedicalTests { get; set; }
        public DbSet<PatientReport> PatientReports { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<EmergencyResponse> EmergencyResponses { get; set; }
        public DbSet<DoctorTimeSlot> DoctorTimeSlots { get; set; }
        public DbSet<MedicalCenter> MedicalCenters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Medical Center - Sectors (1 -> Many)
            modelBuilder.Entity<Sector>()
                .HasOne(s => s.MedicalCenter)
                .WithMany(c => c.Sectors)
                .HasForeignKey(s => s.MedicalCenterId)
                .OnDelete(DeleteBehavior.Cascade);
            // Medical Center - Nurses (1 -> Many)
            modelBuilder.Entity<Nurse>()
                .HasOne(n => n.MedicalCenter)
                .WithMany(mc => mc.Nurses)
                .HasForeignKey(n => n.MedicalCenterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Patient - Appointments (1 -> Many)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Patient - Medical Tests (1 -> Many)
            modelBuilder.Entity<MedicalTest>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalTests)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Patient - Patient Reports (1 -> Many)
            modelBuilder.Entity<PatientReport>()
                .HasOne(r => r.Patient)
                .WithMany(p => p.PatientReports)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Patient - Emergency Response (1 -> Many)
            modelBuilder.Entity<EmergencyResponse>()
                .HasOne(e => e.Patient)
                .WithMany(p => p.EmergencyResponses)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor - Appointments (1 -> Many)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor - Medical Tests (1 -> Many)
            modelBuilder.Entity<MedicalTest>()
                .HasOne(m => m.Doctor)
                .WithMany(d => d.MedicalTests)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Sector - Doctors (1 -> Many)
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Sector)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SectorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Appointment - Patient Report (1 -> 1)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.PatientReport)
                .WithOne()
                .HasForeignKey<Appointment>(a => a.PatientReportId)
                .OnDelete(DeleteBehavior.NoAction);

            // Appointment - TimeSlot (1 -> 1)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.TimeSlot)
                .WithOne()
                .HasForeignKey<Appointment>(a => a.TimeSlotId)
                .OnDelete(DeleteBehavior.Cascade);

            // Patient Report - Sector (1 -> 1)
            modelBuilder.Entity<PatientReport>()
                .HasOne(r => r.Sector)
                .WithOne()
                .HasForeignKey<PatientReport>(r => r.SectorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Patient Report - Nurse (1 -> Many)
            modelBuilder.Entity<PatientReport>()
                .HasOne(pr => pr.Nurse)
                .WithMany(n => n.PatientReports)
                .HasForeignKey(pr => pr.NurseId)
                .OnDelete(DeleteBehavior.NoAction);

            // Emergency Response - Nurse (1 -> Many)
            modelBuilder.Entity<EmergencyResponse>()
                .HasOne(e => e.Nurse)
                .WithMany(n => n.EmergencyResponses)
                .HasForeignKey(e => e.NurseId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure many-to-many relationship for Doctor <-> TimeSlot
            modelBuilder.Entity<DoctorTimeSlot>()
                .HasKey(dts => new { dts.DoctorId, dts.TimeSlotId }); // Composite key

            modelBuilder.Entity<DoctorTimeSlot>()
                .HasOne(dts => dts.Doctor)
                .WithMany(d => d.DoctorTimeSlots)
                .HasForeignKey(dts => dts.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoctorTimeSlot>()
                .HasOne(dts => dts.TimeSlot)
                .WithMany(ts => ts.DoctorTimeSlots)
                .HasForeignKey(dts => dts.TimeSlotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
