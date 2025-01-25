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

        // GET: Patients
        public async Task<IActionResult> Index()
        {
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

    }
}
