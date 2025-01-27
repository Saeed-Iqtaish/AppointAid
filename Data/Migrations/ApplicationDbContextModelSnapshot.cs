﻿// <auto-generated />
using System;
using AppointAid.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AppointAid.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AppointAid.Models.Appointment", b =>
                {
                    b.Property<int>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointmentId"));

                    b.Property<int>("DoctorId")
                        .HasColumnType("int");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<int>("PatientReportId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TimeSlotId")
                        .HasColumnType("int");

                    b.HasKey("AppointmentId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.HasIndex("PatientReportId")
                        .IsUnique();

                    b.HasIndex("TimeSlotId")
                        .IsUnique();

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("AppointAid.Models.Doctor", b =>
                {
                    b.Property<int>("DoctorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DoctorId"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NationalNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SectorId")
                        .HasColumnType("int");

                    b.Property<string>("Specialty")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DoctorId");

                    b.HasIndex("SectorId");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("AppointAid.Models.DoctorTimeSlot", b =>
                {
                    b.Property<int?>("DoctorId")
                        .HasColumnType("int");

                    b.Property<int?>("TimeSlotId")
                        .HasColumnType("int");

                    b.HasKey("DoctorId", "TimeSlotId");

                    b.HasIndex("TimeSlotId");

                    b.ToTable("DoctorTimeSlots");
                });

            modelBuilder.Entity("AppointAid.Models.EmergencyResponse", b =>
                {
                    b.Property<int>("EmergencyResponseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EmergencyResponseId"));

                    b.Property<string>("EmergencyDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmergencyType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsCritical")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NurseId")
                        .HasColumnType("int");

                    b.Property<string>("NurseInstructions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ResponseTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("EmergencyResponseId");

                    b.HasIndex("NurseId");

                    b.HasIndex("PatientId");

                    b.ToTable("EmergencyResponses");
                });

            modelBuilder.Entity("AppointAid.Models.MedicalCenter", b =>
                {
                    b.Property<int>("MedicalCenterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MedicalCenterId"));

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedicalCenterName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MedicalCenterId");

                    b.ToTable("MedicalCenters");
                });

            modelBuilder.Entity("AppointAid.Models.MedicalTest", b =>
                {
                    b.Property<int>("MedicalTestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MedicalTestId"));

                    b.Property<int?>("DoctorId")
                        .HasColumnType("int");

                    b.Property<bool?>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Results")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TestName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("MedicalTestId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.ToTable("MedicalTests");
                });

            modelBuilder.Entity("AppointAid.Models.Nurse", b =>
                {
                    b.Property<int>("NurseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NurseId"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MedicalCenterId")
                        .HasColumnType("int");

                    b.Property<string>("NationalNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NurseId");

                    b.HasIndex("MedicalCenterId");

                    b.ToTable("Nurses");
                });

            modelBuilder.Entity("AppointAid.Models.Patient", b =>
                {
                    b.Property<int>("PatientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PatientId"));

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NationalNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PatientId");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("AppointAid.Models.PatientReport", b =>
                {
                    b.Property<int>("PatientReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PatientReportId"));

                    b.Property<string>("Diagnosis")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAppointmentSet")
                        .HasColumnType("bit");

                    b.Property<int?>("NurseId")
                        .HasColumnType("int");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<string>("PatientSymptoms")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SectorId")
                        .HasColumnType("int");

                    b.Property<int?>("Severity")
                        .HasColumnType("int");

                    b.Property<string>("Treatment")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PatientReportId");

                    b.HasIndex("NurseId");

                    b.HasIndex("PatientId");

                    b.HasIndex("SectorId")
                        .IsUnique()
                        .HasFilter("[SectorId] IS NOT NULL");

                    b.ToTable("PatientReports");
                });

            modelBuilder.Entity("AppointAid.Models.Sector", b =>
                {
                    b.Property<int>("SectorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SectorId"));

                    b.Property<int>("MedicalCenterId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SectorId");

                    b.HasIndex("MedicalCenterId");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("AppointAid.Models.TimeSlot", b =>
                {
                    b.Property<int>("TimeSlotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TimeSlotId"));

                    b.Property<bool>("Availability")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time");

                    b.HasKey("TimeSlotId");

                    b.ToTable("TimeSlots");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("AppointAid.Models.Appointment", b =>
                {
                    b.HasOne("AppointAid.Models.Doctor", "Doctor")
                        .WithMany("Appointments")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppointAid.Models.Patient", "Patient")
                        .WithMany("Appointments")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppointAid.Models.PatientReport", "PatientReport")
                        .WithOne()
                        .HasForeignKey("AppointAid.Models.Appointment", "PatientReportId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AppointAid.Models.TimeSlot", "TimeSlot")
                        .WithOne()
                        .HasForeignKey("AppointAid.Models.Appointment", "TimeSlotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");

                    b.Navigation("Patient");

                    b.Navigation("PatientReport");

                    b.Navigation("TimeSlot");
                });

            modelBuilder.Entity("AppointAid.Models.Doctor", b =>
                {
                    b.HasOne("AppointAid.Models.Sector", "Sector")
                        .WithMany("Doctors")
                        .HasForeignKey("SectorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sector");
                });

            modelBuilder.Entity("AppointAid.Models.DoctorTimeSlot", b =>
                {
                    b.HasOne("AppointAid.Models.Doctor", "Doctor")
                        .WithMany("DoctorTimeSlots")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppointAid.Models.TimeSlot", "TimeSlot")
                        .WithMany("DoctorTimeSlots")
                        .HasForeignKey("TimeSlotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");

                    b.Navigation("TimeSlot");
                });

            modelBuilder.Entity("AppointAid.Models.EmergencyResponse", b =>
                {
                    b.HasOne("AppointAid.Models.Nurse", "Nurse")
                        .WithMany("EmergencyResponses")
                        .HasForeignKey("NurseId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("AppointAid.Models.Patient", "Patient")
                        .WithMany("EmergencyResponses")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Nurse");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("AppointAid.Models.MedicalTest", b =>
                {
                    b.HasOne("AppointAid.Models.Doctor", "Doctor")
                        .WithMany("MedicalTests")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("AppointAid.Models.Patient", "Patient")
                        .WithMany("MedicalTests")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("AppointAid.Models.Nurse", b =>
                {
                    b.HasOne("AppointAid.Models.MedicalCenter", "MedicalCenter")
                        .WithMany("Nurses")
                        .HasForeignKey("MedicalCenterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MedicalCenter");
                });

            modelBuilder.Entity("AppointAid.Models.PatientReport", b =>
                {
                    b.HasOne("AppointAid.Models.Nurse", "Nurse")
                        .WithMany("PatientReports")
                        .HasForeignKey("NurseId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("AppointAid.Models.Patient", "Patient")
                        .WithMany("PatientReports")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppointAid.Models.Sector", "Sector")
                        .WithOne()
                        .HasForeignKey("AppointAid.Models.PatientReport", "SectorId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Nurse");

                    b.Navigation("Patient");

                    b.Navigation("Sector");
                });

            modelBuilder.Entity("AppointAid.Models.Sector", b =>
                {
                    b.HasOne("AppointAid.Models.MedicalCenter", "MedicalCenter")
                        .WithMany("Sectors")
                        .HasForeignKey("MedicalCenterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MedicalCenter");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AppointAid.Models.Doctor", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("DoctorTimeSlots");

                    b.Navigation("MedicalTests");
                });

            modelBuilder.Entity("AppointAid.Models.MedicalCenter", b =>
                {
                    b.Navigation("Nurses");

                    b.Navigation("Sectors");
                });

            modelBuilder.Entity("AppointAid.Models.Nurse", b =>
                {
                    b.Navigation("EmergencyResponses");

                    b.Navigation("PatientReports");
                });

            modelBuilder.Entity("AppointAid.Models.Patient", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("EmergencyResponses");

                    b.Navigation("MedicalTests");

                    b.Navigation("PatientReports");
                });

            modelBuilder.Entity("AppointAid.Models.Sector", b =>
                {
                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("AppointAid.Models.TimeSlot", b =>
                {
                    b.Navigation("DoctorTimeSlots");
                });
#pragma warning restore 612, 618
        }
    }
}
