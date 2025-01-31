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

namespace AppointAid.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in to view this page.";
                return RedirectToAction("Login", "Account");
            }

            var approvedReports = await GetApprovedReports(patientId.Value);
            var approvedTests = await GetApprovedTests(patientId.Value);
            var upcomingAppointments = await GetUpcomingAppointmentsAsync(patientId.Value);

            ViewBag.ApprovedReports = approvedReports;
            ViewBag.ApprovedTests = approvedTests;
            ViewBag.UpcomingAppointments = upcomingAppointments;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUpcomingAppointments()
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
            {
                return Content("<p class='text-danger'>You must be logged in to view your appointments.</p>");
            }

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId.Value
                            && a.TimeSlot.Date > DateTime.UtcNow
                            && a.Status == "Confirmed")
                .OrderBy(a => a.TimeSlot.Date)
                .ToListAsync();

            return PartialView("_UpcomingAppointmentsPartial", appointments);
        }


        [HttpGet]
        public async Task<IActionResult> GetUpcomingTests()
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
                return BadRequest("Session expired. Please log in again.");

            var tests = await _context.MedicalTests
                .Include(mt => mt.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Where(mt => mt.PatientId == patientId.Value && mt.IsApproved == true && mt.ScheduledDate > DateTime.Now)
                .ToListAsync();

            return PartialView("_UpcomingTestsPartial", tests);
        }

        [HttpGet]
        public async Task<IActionResult> ViewMedicalReport(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.PatientReport)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null || appointment.PatientReport == null)
            {
                TempData["ErrorMessage"] = "No report found for the selected appointment.";
                return RedirectToAction("MedicalAppointments");
            }

            return View(appointment.PatientReport);
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment([FromBody] dynamic requestData)
        {
            try
            {
                int appointmentId = (int)requestData.appointmentId;

                var appointment = await _context.Appointments
                    .Include(a => a.PatientReport)
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (appointment == null)
                {
                    return Json(new { success = false, message = "Appointment not found." });
                }

                if (appointment.PatientReport != null)
                {
                    _context.PatientReports.Remove(appointment.PatientReport);
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Appointment canceled successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while canceling the appointment." });
            }
        }

        private async Task<List<PatientReport>> GetApprovedReports(int patientId)
        {
            return await _context.PatientReports
                .Include(pr => pr.Sector)
                .Where(pr => pr.PatientId == patientId && !pr.IsAppointmentSet && pr.Severity.HasValue && pr.SectorId.HasValue)
                .ToListAsync();
        }

        private async Task<List<MedicalTest>> GetApprovedTests(int patientId)
        {
            return await _context.MedicalTests
                .Include(mt => mt.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Where(mt => mt.PatientId == patientId && mt.IsApproved == true && mt.ScheduledDate == null && mt.ScheduledTime == null)
                .ToListAsync();
        }

        private async Task<List<Appointment>> GetUpcomingAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId
                            && a.TimeSlot.Date > DateTime.UtcNow
                            && a.Status == "Confirmed")
                .OrderBy(a => a.TimeSlot.Date)
                .ToListAsync();
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }

        [HttpGet]
        public IActionResult Symptoms()
        {
                var medicalCenters = _context.MedicalCenters
                    .Select(mc => new SelectListItem
                    {
                        Value = mc.MedicalCenterId.ToString(),
                        Text = mc.MedicalCenterName
                    })
                    .ToList();

                var model = new SymptomsViewModel
                {
                    MedicalCenters = medicalCenters
                };

                return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSymptoms(SymptomsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.MedicalCenters = _context.MedicalCenters
                    .Select(mc => new SelectListItem
                    {
                        Value = mc.MedicalCenterId.ToString(),
                        Text = mc.MedicalCenterName
                    })
                    .ToList();
                return View("Symptoms", model);
            }

            var nurses = await _context.Nurses
                .Where(n => n.MedicalCenterId == model.MedicalCenterId)
                .ToListAsync();

            if (!nurses.Any())
            {
                ModelState.AddModelError("", "No nurses are available in the selected medical center.");
                model.MedicalCenters = _context.MedicalCenters
                    .Select(mc => new SelectListItem
                    {
                        Value = mc.MedicalCenterId.ToString(),
                        Text = mc.MedicalCenterName
                    })
                    .ToList();
                return View("Symptoms", model);
            }

            var random = new Random();
            var assignedNurse = nurses[random.Next(nurses.Count)];

            int? patientId = HttpContext.Session.GetInt32("PatientId");

            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in as a patient to submit symptoms.";
                return RedirectToAction("Login", "Account");
            }

            var report = new PatientReport
            {
                PatientId = patientId.Value,
                NurseId = assignedNurse.NurseId,
                PatientSymptoms = model.Symptoms
            };

            _context.PatientReports.Add(report);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your symptoms have been submitted. A nurse will review them shortly.";
            return RedirectToAction("Index", "Patients");
        }

        public async Task<IActionResult> MedicalAppointments()
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in to view your appointments.";
                return RedirectToAction("Login", "Account");
            }

            var upcomingAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId && a.TimeSlot.Date > DateTime.UtcNow && a.Status == "Confirmed")
                .ToListAsync();

            var previousAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId && a.TimeSlot.Date <= DateTime.UtcNow && a.Status == "Completed")
                .ToListAsync();

            var viewModel = new AppointmentsViewModel
            {
                UpcomingAppointments = upcomingAppointments,
                PreviousAppointments = previousAppointments
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> BookAppointment(int reportId)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var report = await _context.PatientReports
                .Include(pr => pr.Sector)
                .FirstOrDefaultAsync(pr => pr.PatientReportId == reportId);

            if (report == null || !report.SectorId.HasValue)
            {
                TempData["ErrorMessage"] = "Invalid or missing report details.";
                return RedirectToAction("Index");
            }

            var doctors = await _context.Doctors
                .Where(d => d.SectorId == report.SectorId.Value)
                .Select(d => new SelectListItem
                {
                    Value = d.DoctorId.ToString(),
                    Text = d.FullName
                })
                .ToListAsync();

            var model = new BookAppointmentViewModel
            {
                ReportId = reportId,
                PatientId = patientId.Value,
                SectorName = report.Sector?.Name,
                Doctors = doctors
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorSchedule(int doctorId)
        {
            var workingDays = await _context.TimeSlots
                .Where(ts => ts.DoctorId == doctorId && ts.Availability == true && ts.Date >= DateTime.Now)
                .Select(ts => ts.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();


            if (workingDays.Any())
            {
                Console.WriteLine($"Working Days for DoctorId {doctorId}: {string.Join(", ", workingDays)}");
            }
            else
            {
                Console.WriteLine($"No working days found for DoctorId {doctorId}");
            }

            return Json(workingDays.Select(d => d.ToString("yyyy-MM-dd")));
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(int doctorId, DateTime selectedDate)
        {
            Console.WriteLine($"Fetching time slots for DoctorId: {doctorId}, Date: {selectedDate:yyyy-MM-dd}");

            var slots = await _context.TimeSlots
                .Where(ts => ts.DoctorId == doctorId && ts.Date.Date == selectedDate.Date && ts.Availability)
                .ToListAsync();

            var availableSlots = new List<object>();

            foreach (var slot in slots)
            {
                var currentTime = slot.StartTime;
                while (currentTime.Add(TimeSpan.FromMinutes(20)) <= slot.EndTime)
                {
                    availableSlots.Add(new
                    {
                        timeSlotId = slot.TimeSlotId,
                        startTime = TimeOnly.FromTimeSpan(currentTime).ToString("hh:mm tt"),
                        endTime = TimeOnly.FromTimeSpan(currentTime.Add(TimeSpan.FromMinutes(20))).ToString("hh:mm tt")
                    });
                    currentTime = currentTime.Add(TimeSpan.FromMinutes(20));
                }
            }

            return Json(availableSlots);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(BookAppointmentViewModel model)
        {
            ModelState.Remove(nameof(model.Doctors));
            ModelState.Remove(nameof(model.TimeSlots));

            if (!ModelState.IsValid)
            {
                model.Doctors = await _context.Doctors
                    .Where(d => d.SectorId == _context.PatientReports
                        .Where(r => r.PatientReportId == model.ReportId)
                        .Select(r => r.SectorId)
                        .FirstOrDefault())
                    .Select(d => new SelectListItem
                    {
                        Value = d.DoctorId.ToString(),
                        Text = d.FullName
                    })
                    .ToListAsync();

                model.TimeSlots = new List<SelectListItem>();
                TempData["ErrorMessage"] = "Invalid appointment details. Please try again.";
                return View("BookAppointment", model);
            }

            var timeSlot = await _context.TimeSlots.FindAsync(model.SelectedTimeSlotId);
            if (timeSlot == null || !timeSlot.Availability)
            {
                TempData["ErrorMessage"] = "The selected time slot is unavailable.";
                return RedirectToAction("BookAppointment", new { reportId = model.ReportId });
            }

            var doctor = await _context.Doctors.FindAsync(model.SelectedDoctorId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "The selected doctor does not exist.";
                return RedirectToAction("BookAppointment", new { reportId = model.ReportId });
            }

            timeSlot.Availability = false;

            var report = await _context.PatientReports.FindAsync(model.ReportId);
            if (report != null)
            {
                report.IsAppointmentSet = true; 
                _context.PatientReports.Update(report);
            }

            var appointment = new Appointment
            {
                PatientId = model.PatientId,
                DoctorId = model.SelectedDoctorId,
                PatientReportId = model.ReportId,
                TimeSlotId = model.SelectedTimeSlotId,
                Status = "Confirmed"
            };

            _context.Appointments.Add(appointment);

            await _context.SaveChangesAsync();

            TempData["Message"] = "Your appointment has been successfully booked!";
            return RedirectToAction("MedicalAppointments");
        }

        [HttpGet]
        public IActionResult RequestMedicalTest()
        {
            var medicalCenters = _context.MedicalCenters
                .Select(mc => new SelectListItem
                {
                    Value = mc.MedicalCenterId.ToString(),
                    Text = mc.MedicalCenterName
                })
                .ToList();

            var model = new RequestMedicalTestViewModel
            {
                MedicalCenters = medicalCenters
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitMedicalTestRequest(RequestMedicalTestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.MedicalCenters = await _context.MedicalCenters
                    .Select(mc => new SelectListItem
                    {
                        Value = mc.MedicalCenterId.ToString(),
                        Text = mc.MedicalCenterName
                    })
                    .ToListAsync();
                return View("RequestMedicalTest", model);
            }

            var doctor = await _context.Doctors
                .Where(d => d.Sector.MedicalCenterId == model.MedicalCenterId)
                .FirstOrDefaultAsync();

            if (doctor == null)
            {
                ModelState.AddModelError("", "No doctors available in the selected medical center.");
                model.MedicalCenters = await _context.MedicalCenters
                    .Select(mc => new SelectListItem
                    {
                        Value = mc.MedicalCenterId.ToString(),
                        Text = mc.MedicalCenterName
                    })
                    .ToListAsync();
                return View("RequestMedicalTest", model);
            }

            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in as a patient to request a medical test.";
                return RedirectToAction("Login", "Account");
            }

            var medicalTest = new MedicalTest
            {
                PatientId = patientId.Value,
                DoctorId = doctor.DoctorId,
                TestName = model.TestName,
                Reason = model.Reason,
                ScheduledDate = null,
                ScheduledTime = null,
                IsApproved = null,
                Results = null
            };

            _context.MedicalTests.Add(medicalTest);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your test request has been submitted and is awaiting approval.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ScheduleTest(int testId)
        {
            var test = await _context.MedicalTests
                .Include(mt => mt.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .FirstOrDefaultAsync(mt => mt.MedicalTestId == testId);

            if (test == null || test.IsApproved != true)
            {
                TempData["ErrorMessage"] = "Invalid or unapproved test.";
                return RedirectToAction("Index");
            }

            ViewBag.TestName = test.TestName;
            ViewBag.MedicalCenterName = test.Doctor?.Sector?.MedicalCenter?.MedicalCenterName;

            var model = new ScheduleTestViewModel
            {
                TestId = test.MedicalTestId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleTest(ScheduleTestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data. Please correct the errors.";
                return View(model);
            }

            var test = await _context.MedicalTests.FirstOrDefaultAsync(mt => mt.MedicalTestId == model.TestId);
            if (test == null || test.IsApproved != true)
            {
                TempData["ErrorMessage"] = "Invalid or unapproved test.";
                return RedirectToAction("Index");
            }

            try
            {
                test.ScheduledDate = model.ScheduledDate;
                test.ScheduledTime = model.ScheduledTime;

                _context.MedicalTests.Update(test);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Test scheduled successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to schedule the test.";
                Console.WriteLine($"Error: {ex.Message}");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> MedicalTests()
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in to view your medical tests.";
                return RedirectToAction("Login", "Account");
            }

            var upcomingTests = await _context.MedicalTests
                .Include(mt => mt.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Where(mt => mt.PatientId == patientId.Value
                             && mt.IsApproved == true
                             && mt.ScheduledDate > DateTime.Now
                             && mt.Results == null)
                .ToListAsync();

            var previousTests = await _context.MedicalTests
                .Include(mt => mt.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Where(mt => mt.PatientId == patientId.Value
                             && mt.IsApproved == true
                             && mt.Results != null)
                .ToListAsync();

            ViewBag.UpcomingTests = upcomingTests;
            ViewBag.PreviousTests = previousTests;

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Emergency(int? id)
        {
            if (id.HasValue)
            {
                var emergency = await _context.EmergencyResponses
                    .Include(er => er.Nurse)
                    .Include(er => er.Patient)
                    .FirstOrDefaultAsync(er => er.EmergencyResponseId == id);

                if (emergency == null)
                {
                    TempData["ErrorMessage"] = "Emergency record not found.";
                    return RedirectToAction("Index");
                }

                return View(emergency);
            }

            return View(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Emergency(string emergencyType, string description, string location)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in to request emergency assistance.";
                return RedirectToAction("Login", "Account");
            }

            var nurses = await _context.Nurses.ToListAsync();
            var random = new Random();
            var assignedNurse = nurses[random.Next(nurses.Count)];

            var emergencyResponse = new EmergencyResponse
            {
                PatientId = patientId.Value,
                NurseId = assignedNurse.NurseId,
                EmergencyType = emergencyType,
                Location = location,
                Time = DateTime.Now,
                Status = "Pending Review"
            };

            _context.EmergencyResponses.Add(emergencyResponse);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your emergency request has been submitted and is pending review.";
            return RedirectToAction("Emergency", new { id = emergencyResponse.EmergencyResponseId });
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
                AppointmentId = appointment.AppointmentId,
                DoctorId = appointment.DoctorId,
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
                TempData["ErrorMessage"] = "Invalid input. Please try again.";
                return View(model);
            }

            var appointment = await _context.Appointments
                .Include(a => a.TimeSlot)
                .FirstOrDefaultAsync(a => a.AppointmentId == model.AppointmentId);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Index");
            }

            var newTimeSlot = await _context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == model.SelectedTimeSlotId && ts.Availability);

            if (newTimeSlot == null)
            {
                TempData["ErrorMessage"] = "The selected time slot is unavailable.";
                return RedirectToAction("Reschedule", new { appointmentId = model.AppointmentId });
            }

            if (appointment.TimeSlot != null)
            {
                appointment.TimeSlot.Availability = true;
            }

            newTimeSlot.Availability = false;
            appointment.TimeSlotId = newTimeSlot.TimeSlotId;

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Appointment rescheduled successfully!";
            return RedirectToAction("Index");
        }
    }
}