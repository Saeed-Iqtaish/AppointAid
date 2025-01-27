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

    public class NursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NursesController(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            int? nurseId = HttpContext.Session.GetInt32("NurseId");
            if (!nurseId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in as a nurse to view this page.";
                return RedirectToAction("Login", "Account");
            }

            // Fetch reports assigned to the logged-in nurse where sector and severity are not assigned
            var reports = await _context.PatientReports
                .Include(pr => pr.Patient)
                .Where(pr => pr.NurseId == nurseId && pr.SectorId == null && !pr.Severity.HasValue)
                .ToListAsync();

            return View(reports);
        }

        // GET: Review Report
        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            var report = await _context.PatientReports
                .Include(pr => pr.Patient)
                .FirstOrDefaultAsync(pr => pr.PatientReportId == id);

            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction(nameof(Index));
            }

            var sectors = await _context.Sectors
                .Select(s => new SelectListItem
                {
                    Value = s.SectorId.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            ViewBag.Sectors = sectors;
            return View(report);
        }

        // POST: Submit Review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int id, int sectorId, int severity, string comments)
        {
            var report = await _context.PatientReports.FindAsync(id);
            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction(nameof(Index));
            }

            report.SectorId = sectorId;
            report.Severity = severity;
            report.Treatment = comments;

            _context.PatientReports.Update(report);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Patient report reviewed successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Nurses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurse = await _context.Nurses
                .FirstOrDefaultAsync(m => m.NurseId == id);
            if (nurse == null)
            {
                return NotFound();
            }

            return View(nurse);
        }

        // GET: Nurses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nurses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NurseId,FirstName,LastName,NationalNumber,PasswordHash")] Nurse nurse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nurse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nurse);
        }

        // GET: Nurses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurse = await _context.Nurses.FindAsync(id);
            if (nurse == null)
            {
                return NotFound();
            }
            return View(nurse);
        }

        // POST: Nurses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NurseId,FirstName,LastName,NationalNumber,PasswordHash")] Nurse nurse)
        {
            if (id != nurse.NurseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nurse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NurseExists(nurse.NurseId))
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
            return View(nurse);
        }

        // GET: Nurses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurse = await _context.Nurses
                .FirstOrDefaultAsync(m => m.NurseId == id);
            if (nurse == null)
            {
                return NotFound();
            }

            return View(nurse);
        }

        // POST: Nurses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nurse = await _context.Nurses.FindAsync(id);
            if (nurse != null)
            {
                _context.Nurses.Remove(nurse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NurseExists(int id)
        {
            return _context.Nurses.Any(e => e.NurseId == id);
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
        public async Task<IActionResult> EmergencyRequests()
        {
            int? nurseId = HttpContext.Session.GetInt32("NurseId");
            if (!nurseId.HasValue)
            {
                TempData["ErrorMessage"] = "You must be logged in as a nurse to view this page.";
                return RedirectToAction("Login", "Account");
            }

            var emergencyRequests = await _context.EmergencyResponses
                .Include(er => er.Patient)
                .Where(er => er.NurseId == nurseId && er.Status == "Pending Review")
                .ToListAsync();

            return View(emergencyRequests);
        }

        [HttpGet]
        public async Task<IActionResult> ReviewEmergency(int id)
        {
            var emergency = await _context.EmergencyResponses
                .Include(er => er.Patient)
                .FirstOrDefaultAsync(er => er.EmergencyResponseId == id);

            if (emergency == null)
            {
                TempData["ErrorMessage"] = "Emergency request not found.";
                return RedirectToAction("EmergencyRequests");
            }

            return View(emergency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewEmergency(int id, string comments, string status)
        {
            var emergency = await _context.EmergencyResponses.FindAsync(id);
            if (emergency == null)
            {
                TempData["ErrorMessage"] = "Emergency request not found.";
                return RedirectToAction("EmergencyRequests");
            }

            emergency.NurseInstructions = comments;
            emergency.Status = status;

            _context.EmergencyResponses.Update(emergency);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Emergency request has been updated successfully.";
            return RedirectToAction("EmergencyRequests");
        }
    }
}
