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

            var reports = await _context.PatientReports
                .Include(pr => pr.Patient)
                .Where(pr => pr.NurseId == nurseId && pr.SectorId == null && !pr.Severity.HasValue)
                .ToListAsync();

            return View(reports);
        }

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