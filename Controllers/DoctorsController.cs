using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppointAid.Data;
using AppointAid.Models;
using AppointAid.ViewModels;
using AppointAid.Services;

namespace AppointAid.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (!doctorId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in as a doctor to view this page.";
                return RedirectToAction("Login", "Account");
            }

            var requestedTests = await _context.MedicalTests
                .Include(mt => mt.Patient)
                .Where(mt => mt.DoctorId == doctorId && mt.IsApproved == null) 
                .ToListAsync();

            var upcomingAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.TimeSlot)
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == doctorId
                            && a.TimeSlot.Date > DateTime.Now
                            && a.Status == "Confirmed")
                .ToListAsync();

            ViewBag.RequestedTests = requestedTests;
            ViewBag.UpcomingAppointments = upcomingAppointments;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PatientHistory(int id)
        {
            var patient = await GetPatientAsync(id);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction("Index");
            }

            var appointments = await GetAppointmentsAsync(id);
            var medicalTests = await GetMedicalTestsAsync(id);

            var model = new PatientHistoryViewModel
            {
                Patient = patient,
                Appointments = appointments,
                MedicalTests = medicalTests
            };

            return View(model);
        }

        private async Task<Patient> GetPatientAsync(int patientId)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        private async Task<List<Appointment>> GetAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        private async Task<List<MedicalTest>> GetMedicalTestsAsync(int patientId)
        {
            return await _context.MedicalTests
                .Where(mt => mt.PatientId == patientId)
                .ToListAsync();
        }


        [HttpGet]
        public async Task<IActionResult> RequestedTests()
        {
            var tests = await _context.MedicalTests
                .Include(mt => mt.Patient)
                .Where(mt => mt.IsApproved == null)
                .ToListAsync();

            return View(tests);
        }


        [HttpGet]
        public async Task<IActionResult> ReviewTest(int id)
        {
            var test = await _context.MedicalTests
                .Include(mt => mt.Patient)
                .FirstOrDefaultAsync(mt => mt.MedicalTestId == id);

            if (test == null)
            {
                TempData["ErrorMessage"] = "Test request not found.";
                return RedirectToAction("RequestedTests");
            }

            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewTest(int id, bool approve)
        {
            var test = await _context.MedicalTests.FindAsync(id);
            if (test == null)
            {
                TempData["ErrorMessage"] = "Test not found.";
                return RedirectToAction("RequestedTests");
            }

            test.IsApproved = approve;

            if (approve)
            {
                test.ScheduledDate = null;
                test.ScheduledTime = null;
            }

            _context.MedicalTests.Update(test);
            await _context.SaveChangesAsync();

            TempData["Message"] = approve ? "Test approved successfully." : "Test denied successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SetSchedule()
        {
            var startDate = DateTime.Now.Date.AddDays(7 - (int)DateTime.Now.DayOfWeek);
            var endDate = startDate.AddDays(6);

            var viewModel = new WeeklyScheduleViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Days = Enum.GetNames(typeof(DayOfWeek)).Select(day => new ScheduleDay
                {
                    Day = day,
                    IsAvailable = false,
                    StartTime = null,
                    EndTime = null
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetSchedule(WeeklyScheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int? doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (!doctorId.HasValue)
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var existingTimeSlots = await _context.TimeSlots
                .Where(ts => ts.DoctorId == doctorId && ts.Date >= model.StartDate && ts.Date <= model.EndDate)
                .ToListAsync();

            _context.TimeSlots.RemoveRange(existingTimeSlots);
            await _context.SaveChangesAsync();

            foreach (var day in model.Days.Where(d => d.IsAvailable))
            {
                for (var date = model.StartDate; date <= model.EndDate; date = date.AddDays(1))
                {
                    if (Enum.GetName(typeof(DayOfWeek), date.DayOfWeek) == day.Day)
                    {
                        var currentTime = day.StartTime.Value;
                        while (currentTime.Add(TimeSpan.FromMinutes(20)) <= day.EndTime.Value)
                        {
                            var timeSlot = new TimeSlot
                            {
                                DoctorId = doctorId.Value,
                                Date = date,
                                StartTime = currentTime,
                                EndTime = currentTime.Add(TimeSpan.FromMinutes(20)),
                                Availability = true
                            };

                            _context.TimeSlots.Add(timeSlot);
                            currentTime = currentTime.Add(TimeSpan.FromMinutes(20));
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Schedule saved successfully!";
            return RedirectToAction("Index", "Doctors");
        }

        [HttpGet]
        public async Task<IActionResult> ViewPatientReport(int reportId)
        {
            var report = await _context.PatientReports
                .Include(r => r.Patient)
                .Include(r => r.Sector)
                .FirstOrDefaultAsync(r => r.PatientReportId == reportId);

            if (report == null)
            {
                TempData["ErrorMessage"] = "Patient report not found.";
                return RedirectToAction("Index");
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePatientReport(PatientReport report)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data. Please try again.";
                return View("ViewPatientReport", report);
            }

            var existingReport = await _context.PatientReports.FindAsync(report.PatientReportId);
            if (existingReport == null)
            {
                TempData["ErrorMessage"] = "Patient report not found.";
                return RedirectToAction("Index");
            }

            existingReport.Diagnosis = report.Diagnosis;
            existingReport.Treatment = report.Treatment;

            var associatedAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.PatientReportId == report.PatientReportId);

            if (associatedAppointment != null)
            {
                associatedAppointment.Status = "Completed";
                _context.Appointments.Update(associatedAppointment);
            }

            _context.PatientReports.Update(existingReport);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Patient report and appointment updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Reschedule(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Sector)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Index");
            }

            var model = new RescheduleAppointmentViewModel
            {
                AppointmentId = appointmentId,
                SectorName = appointment.Doctor?.Sector?.Name ?? "Unknown"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule(RescheduleAppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data.";
                return View(model);
            }

            var appointment = await _context.Appointments.FindAsync(model.AppointmentId);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Index");
            }

            var timeSlot = await _context.TimeSlots.FindAsync(model.SelectedTimeSlotId);
            if (timeSlot == null || !timeSlot.Availability)
            {
                TempData["ErrorMessage"] = "The selected time slot is unavailable.";
                return RedirectToAction("Reschedule", new { id = model.AppointmentId });
            }

            timeSlot.Availability = false;
            appointment.TimeSlotId = model.SelectedTimeSlotId;

            await _context.SaveChangesAsync();

            TempData["Message"] = "Appointment rescheduled successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(DateTime selectedDate)
        {
            var slots = await _context.TimeSlots
                .Where(ts => ts.Date.Date == selectedDate.Date && ts.Availability)
                .Select(ts => new
                {
                    timeSlotId = ts.TimeSlotId,
                    startTime = ts.StartTime.ToString("hh:mm tt"),
                    endTime = ts.EndTime.ToString("hh:mm tt")
                })
                .ToListAsync();

            return Json(slots);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.TimeSlot)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Index");
            }

            if (appointment.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "The appointment is already cancelled.";
                return RedirectToAction("Index");
            }

            appointment.Status = "Cancelled";
            if (appointment.TimeSlot != null)
            {
                appointment.TimeSlot.Availability = true;
            }

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Appointment cancelled successfully.";
            return RedirectToAction("Index");
        }
    }
}