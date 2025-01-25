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
    public class PatientReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PatientReports
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PatientReports.Include(p => p.Nurse).Include(p => p.Patient).Include(p => p.Sector);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PatientReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientReport = await _context.PatientReports
                .Include(p => p.Nurse)
                .Include(p => p.Patient)
                .Include(p => p.Sector)
                .FirstOrDefaultAsync(m => m.PatientReportId == id);
            if (patientReport == null)
            {
                return NotFound();
            }

            return View(patientReport);
        }

        // GET: PatientReports/Create
        public IActionResult Create()
        {
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "FirstName");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName");
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "SectorId");
            return View();
        }

        // POST: PatientReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientReportId,PatientId,NurseId,SectorId,PatientSymptoms,Severity,Diagnosis,Treatment")] PatientReport patientReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "FirstName", patientReport.NurseId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", patientReport.PatientId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "SectorId", patientReport.SectorId);
            return View(patientReport);
        }

        // GET: PatientReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientReport = await _context.PatientReports.FindAsync(id);
            if (patientReport == null)
            {
                return NotFound();
            }
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "FirstName", patientReport.NurseId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", patientReport.PatientId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "SectorId", patientReport.SectorId);
            return View(patientReport);
        }

        // POST: PatientReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientReportId,PatientId,NurseId,SectorId,PatientSymptoms,Severity,Diagnosis,Treatment")] PatientReport patientReport)
        {
            if (id != patientReport.PatientReportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientReportExists(patientReport.PatientReportId))
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
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "FirstName", patientReport.NurseId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", patientReport.PatientId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "SectorId", patientReport.SectorId);
            return View(patientReport);
        }

        // GET: PatientReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientReport = await _context.PatientReports
                .Include(p => p.Nurse)
                .Include(p => p.Patient)
                .Include(p => p.Sector)
                .FirstOrDefaultAsync(m => m.PatientReportId == id);
            if (patientReport == null)
            {
                return NotFound();
            }

            return View(patientReport);
        }

        // POST: PatientReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientReport = await _context.PatientReports.FindAsync(id);
            if (patientReport != null)
            {
                _context.PatientReports.Remove(patientReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientReportExists(int id)
        {
            return _context.PatientReports.Any(e => e.PatientReportId == id);
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

            // Check if there are no medical centers
            if (!medicalCenters.Any())
            {
                TempData["ErrorMessage"] = "No medical centers are currently available. Please try again later.";
                return RedirectToAction("Index", "Home"); // Redirect to a fallback page
            }

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

            int? patientId = HttpContext.Session.GetInt32("PatientId");

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
    }
}
