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

            var approvedReports = await _context.PatientReports
                .Include(pr => pr.Sector)
                .Where(pr => pr.PatientId == patientId && pr.SectorId != null)
                .ToListAsync();

            ViewBag.ApprovedReports = approvedReports;

            return View();
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,FirstName,LastName,NationalNumber,DateOfBirth,PhoneNumber,PasswordHash")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,FirstName,LastName,NationalNumber,DateOfBirth,PhoneNumber,PasswordHash")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }

        [HttpGet]
        public IActionResult Symptoms()
        {
                // Fetch all medical centers
                var medicalCenters = _context.MedicalCenters
                    .Select(mc => new SelectListItem
                    {
                        Value = mc.MedicalCenterId.ToString(),
                        Text = mc.MedicalCenterName
                    })
                    .ToList();

                // Prepare the model
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

            // Fetch nurses from the selected medical center
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

            // Select a random nurse
            var random = new Random();
            var assignedNurse = nurses[random.Next(nurses.Count)];

            // Get the patient ID from session
            int? patientId = HttpContext.Session.GetInt32("PatientId");

            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in as a patient to submit symptoms.";
                return RedirectToAction("Login", "Account");
            }

            // Create the patient report
            var report = new PatientReport
            {
                PatientId = patientId.Value,
                NurseId = assignedNurse.NurseId,
                PatientSymptoms = model.Symptoms
            };

            _context.PatientReports.Add(report);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your symptoms have been submitted. A nurse will review them shortly.";
            return RedirectToAction("Index", "Home");
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
                .Where(a => a.PatientId == patientId && a.TimeSlot.Date > DateTime.Now)
                .ToListAsync();

            var previousAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId && a.TimeSlot.Date <= DateTime.Now)
                .ToListAsync();

            ViewBag.UpcomingAppointments = upcomingAppointments;
            ViewBag.PreviousAppointments = previousAppointments;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> BookAppointment(int reportId)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "Session has expired. Please log in again.";
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
                PatientId = patientId.Value,
                ReportId = reportId,
                SectorId = report.SectorId.Value,
                SectorName = report.Sector?.Name ?? "Unknown",
                Doctors = doctors
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(BookAppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid appointment details.";
                return RedirectToAction("Index");
            }

            var timeSlot = await _context.TimeSlots
                .Include(ts => ts.DoctorTimeSlots)
                .FirstOrDefaultAsync(ts => ts.Date == model.PreferredDate &&
                                            ts.StartTime == model.PreferredTime &&
                                            ts.DoctorTimeSlots.Any(dts => dts.DoctorId == model.SelectedDoctorId) &&
                                            ts.Availability);

            if (timeSlot == null)
            {
                TempData["ErrorMessage"] = "The selected time slot is unavailable.";
                return RedirectToAction("BookAppointment", new { reportId = model.ReportId });
            }

            timeSlot.Availability = false;

            var appointment = new Appointment
            {
                PatientId = model.PatientId,
                DoctorId = model.SelectedDoctorId,
                PatientReportId = model.ReportId,
                TimeSlotId = timeSlot.TimeSlotId,
                Status = "Confirmed"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your appointment has been successfully booked!";
            return RedirectToAction("MedicalAppointments");
        }


        [HttpGet]
        public async Task<JsonResult> GetAvailableTimes(int doctorId, DateTime date)
        {
            var times = await _context.DoctorTimeSlots
                .Include(dts => dts.TimeSlot)
                .Where(dts => dts.DoctorId == doctorId && dts.TimeSlot.Availability && dts.TimeSlot.Date == date)
                .Select(dts => dts.TimeSlot.StartTime)
                .ToListAsync();

            return Json(times.Select(t => new { Time = t.ToString(@"hh\:mm") }));
        }

        [HttpGet]
        public async Task<IActionResult> RequestMedicalTest()
        {
            var medicalCenters = await _context.MedicalCenters
                .Select(mc => new SelectListItem
                {
                    Value = mc.MedicalCenterId.ToString(),
                    Text = mc.MedicalCenterName
                })
                .ToListAsync();

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

            // Get the doctor from the selected medical center
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

            // Get the logged-in patient's ID
            int? patientId = HttpContext.Session.GetInt32("PatientId");
            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in as a patient to request a medical test.";
                return RedirectToAction("Login", "Account");
            }

            // Create a new medical test request
            var medicalTest = new MedicalTest
            {
                PatientId = patientId.Value,
                DoctorId = doctor.DoctorId,
                TestName = model.TestName,
                Reason = model.Reason,
                Time = DateTime.Now,
                IsApproved = null,
                Results = null
            };

            _context.MedicalTests.Add(medicalTest);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your test request has been submitted and is awaiting approval.";
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

            // Fetch upcoming approved tests
            var upcomingTests = await _context.MedicalTests
                .Include(mt => mt.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Where(mt => mt.PatientId == patientId.Value && mt.IsApproved == true && mt.Time > DateTime.Now)
                .ToListAsync();

            // Fetch previous approved tests
            var previousTests = await _context.MedicalTests
                .Include(mt => mt.Doctor)
                .ThenInclude(d => d.Sector)
                .ThenInclude(s => s.MedicalCenter)
                .Where(mt => mt.PatientId == patientId.Value && mt.IsApproved == true && mt.Time <= DateTime.Now)
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
                // Fetch the emergency details if the ID is provided
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

            // If no ID is provided, render the request form
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

            // Randomly assign a nurse for the prototype
            var nurses = await _context.Nurses.ToListAsync();
            var random = new Random();
            var assignedNurse = nurses[random.Next(nurses.Count)];

            // Create a new emergency response
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

    }
}
